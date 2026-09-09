using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Enums;

namespace Testhon.Framework.Driver;

/// <summary>
/// Owns the Playwright lifecycle for a single test. Supports desktop, Android emulation and a
/// real Android device (via ADB). Captures trace/video/screenshot artifacts per configuration.
/// One instance per test keeps parallel runs fully isolated.
/// </summary>
public sealed class PlaywrightDriver : ITestSession
{
    private readonly ILogger _log;
    private readonly RunSettings _settings;
    private IPlaywright? _playwright;
    private string? _videoDir;
    private bool _tracingStarted;
    private bool _connectedOverCdp;
    private bool _disposed;

    public IBrowser? Browser { get; private set; }
    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;

    public PlaywrightDriver(ILogger log, RunSettings settings)
    {
        _log = log;
        _settings = settings;
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();

        if (_settings.Platform == Platform.RealAndroid)
        {
            await InitializeRealAndroidAsync();
        }
        else
        {
            await InitializeBrowserAsync();
        }

        Context.SetDefaultTimeout(_settings.DefaultTimeoutMs);
        Context.SetDefaultNavigationTimeout(_settings.NavigationTimeoutMs);

        // Tracing over a CDP-connected context is not supported, so skip it for real devices.
        if (_settings.Trace != CaptureMode.Off && _settings.Platform != Platform.RealAndroid)
        {
            await Context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            _tracingStarted = true;
        }

        _log.Information(
            "Session started | Platform={Platform} | Browser={Browser} | Device={Device} | Headless={Headless}",
            _settings.Platform, _settings.Browser, _settings.DeviceName, _settings.Headless);
    }

    private async Task InitializeBrowserAsync()
    {
        Browser = await BrowserFactory.LaunchAsync(_playwright!, _settings, _log);
        var options = BrowserFactory.BuildContextOptions(_playwright!, _settings);

        if (_settings.Video != CaptureMode.Off)
        {
            options.RecordVideoDir = CreateVideoDir();
        }

        Context = await Browser.NewContextAsync(options);
        Page = await Context.NewPageAsync();
    }

    private async Task InitializeRealAndroidAsync()
    {
        // The .NET Playwright binding has no native Android API, so we drive Chrome on the
        // device via the Chrome DevTools Protocol tunneled over ADB.
        var endpoint = string.IsNullOrWhiteSpace(_settings.AndroidCdpEndpoint)
            ? "http://localhost:9222"
            : _settings.AndroidCdpEndpoint;

        if (_settings.AutoAdbForward && Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            TryAdbForward(uri.Port);
        }

        _log.Information("Connecting to Chrome on Android over CDP at {Endpoint}", endpoint);
        Browser = await _playwright!.Chromium.ConnectOverCDPAsync(endpoint);
        _connectedOverCdp = true;

        Context = Browser.Contexts.Count > 0 ? Browser.Contexts[0] : await Browser.NewContextAsync();
        Page = Context.Pages.Count > 0 ? Context.Pages[0] : await Context.NewPageAsync();
    }

    private void TryAdbForward(int port)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("adb",
                $"forward tcp:{port} localabstract:chrome_devtools_remote")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(psi);
            process?.WaitForExit(5000);
            _log.Information("adb forward tcp:{Port} -> chrome_devtools_remote established", port);
        }
        catch (Exception ex)
        {
            _log.Warning("Could not run 'adb forward' automatically ({Message}). " +
                "Ensure adb is on PATH or forward the port manually.", ex.Message);
        }
    }

    /// <summary>
    /// Ends the session and collects artifacts: screenshot (on failure), trace and video
    /// according to their <see cref="CaptureMode"/>. Closes the browser/device.
    /// </summary>
    public async Task<SessionArtifacts> FinalizeAsync(bool testFailed, string testName)
    {
        var keepTrace = _settings.Trace == CaptureMode.Always || (_settings.Trace == CaptureMode.OnFailure && testFailed);
        var keepVideo = _settings.Video == CaptureMode.Always || (_settings.Video == CaptureMode.OnFailure && testFailed);

        string? screenshotBase64 = null;
        string? tracePath = null;
        string? videoPath = null;

        // Screenshot requires an open page.
        if (testFailed && _settings.Screenshot.CaptureOnFailure)
        {
            screenshotBase64 = await CaptureScreenshotAsync(testName);
        }

        // Tracing must be stopped before the context is closed.
        if (_tracingStarted)
        {
            if (keepTrace)
            {
                tracePath = BuildArtifactPath("traces", testName, "zip");
                await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                _log.Information("Trace saved: {Path}", tracePath);
            }
            else
            {
                await Context.Tracing.StopAsync();
            }

            _tracingStarted = false;
        }

        // A video is only finalized after its context closes; keep the Playwright connection
        // alive until we've saved/deleted the file, then tear everything down.
        var video = Page.Video;

        if (!_connectedOverCdp && Context is not null)
        {
            await Context.CloseAsync();
        }

        if (video is not null)
        {
            if (keepVideo)
            {
                videoPath = BuildArtifactPath("videos", testName, "webm");
                await video.SaveAsAsync(videoPath);
                _log.Information("Video saved: {Path}", videoPath);
            }

            await video.DeleteAsync();
        }

        await DisposeAsync();
        CleanupVideoDir();
        return new SessionArtifacts(screenshotBase64, tracePath, videoPath);
    }

    /// <summary>Captures a screenshot, persists it under reports/screenshots and returns its Base64.</summary>
    public async Task<string> CaptureScreenshotAsync(string testName)
    {
        var bytes = await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = _settings.Screenshot.FullPage
        });

        var file = BuildArtifactPath("screenshots", testName, "png");
        await File.WriteAllBytesAsync(file, bytes);

        _log.Debug("Screenshot saved to {File}", file);
        return Convert.ToBase64String(bytes);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            if (_connectedOverCdp)
            {
                // Disconnect Playwright but leave the device's Chrome running.
                if (Browser is not null)
                {
                    await Browser.CloseAsync();
                }
            }
            else
            {
                if (Context is not null)
                {
                    await Context.CloseAsync();
                }

                if (Browser is not null)
                {
                    await Browser.CloseAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _log.Warning("Error while disposing session: {Message}", ex.Message);
        }
        finally
        {
            _playwright?.Dispose();
        }
    }

    private string CreateVideoDir()
    {
        _videoDir = Path.Combine(Path.GetTempPath(), "testhon-videos", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_videoDir);
        return _videoDir;
    }

    private void CleanupVideoDir()
    {
        if (_videoDir is null || !Directory.Exists(_videoDir))
        {
            return;
        }

        try
        {
            Directory.Delete(_videoDir, recursive: true);
        }
        catch
        {
            // Best-effort temp cleanup.
        }
    }

    private static string BuildArtifactPath(string subFolder, string testName, string extension)
    {
        var dir = Path.Combine(FrameworkConfig.ResultsDirectory, subFolder);
        Directory.CreateDirectory(dir);

        var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(dir, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.{extension}");
    }
}
