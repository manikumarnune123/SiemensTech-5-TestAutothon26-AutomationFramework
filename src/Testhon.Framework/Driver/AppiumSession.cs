using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Enums;

namespace Testhon.Framework.Driver;

/// <summary>
/// Owns the Appium lifecycle for a single test: builds capabilities from <see cref="AppiumSettings"/>,
/// starts a native (APK/IPA) or mobile-web session, records the screen and captures a screenshot on
/// failure. One instance per test keeps parallel runs isolated. Named <c>AppiumSession</c> (not
/// <c>AppiumDriver</c>) to avoid clashing with the Appium client's own <see cref="AppiumDriver"/>.
/// </summary>
public sealed class AppiumSession : ITestSession
{
    private readonly ILogger _log;
    private readonly RunSettings _settings;
    private bool _recording;
    private bool _disposed;

    /// <summary>The live Appium WebDriver session (Android or iOS).</summary>
    public AppiumDriver Driver { get; private set; } = null!;

    public AppiumSession(ILogger log, RunSettings settings)
    {
        _log = log;
        _settings = settings;
    }

    public async Task InitializeAsync()
    {
        var appium = _settings.Appium;
        var options = BuildOptions(appium);
        var server = new Uri(appium.ServerUrl);
        var commandTimeout = TimeSpan.FromSeconds(appium.CommandTimeoutSec);

        _log.Information(
            "Starting Appium session | Server={Server} | Platform={Platform} | Automation={Automation} | Device={Device}",
            appium.ServerUrl, appium.PlatformName, appium.AutomationName, appium.DeviceName);

        Driver = await Task.Run(() => appium.PlatformName.Equals("iOS", StringComparison.OrdinalIgnoreCase)
            ? new IOSDriver(server, options, commandTimeout)
            : (AppiumDriver)new AndroidDriver(server, options, commandTimeout));

        // Mobile-web run: drive a real mobile browser to the target URL.
        if (!string.IsNullOrWhiteSpace(appium.BrowserName) && !string.IsNullOrWhiteSpace(appium.BaseUrl))
        {
            _log.Information("Navigating mobile browser to {Url}", appium.BaseUrl);
            await Task.Run(() => Driver.Navigate().GoToUrl(appium.BaseUrl));
        }

        if (_settings.Video != CaptureMode.Off)
        {
            TryStartRecording();
        }

        _log.Information("Appium session ready");
    }

    private AppiumOptions BuildOptions(AppiumSettings appium)
    {
        var options = new AppiumOptions
        {
            PlatformName = appium.PlatformName,
            AutomationName = appium.AutomationName,
            DeviceName = appium.DeviceName
        };

        if (!string.IsNullOrWhiteSpace(appium.PlatformVersion))
        {
            options.PlatformVersion = appium.PlatformVersion;
        }

        if (!string.IsNullOrWhiteSpace(appium.BrowserName))
        {
            // Mobile-web: no app is installed; Appium launches the browser instead.
            options.BrowserName = appium.BrowserName;
        }
        else
        {
            var app = ResolveAppPath(appium.App);
            if (!string.IsNullOrWhiteSpace(app))
            {
                options.App = app;
                _log.Information("Using app: {App}", app);
            }

            if (!string.IsNullOrWhiteSpace(appium.AppPackage))
            {
                options.AddAdditionalAppiumOption("appPackage", appium.AppPackage);
            }

            if (!string.IsNullOrWhiteSpace(appium.AppActivity))
            {
                options.AddAdditionalAppiumOption("appActivity", appium.AppActivity);
            }

            options.AddAdditionalAppiumOption("autoGrantPermissions", appium.AutoGrantPermissions);
        }

        if (!string.IsNullOrWhiteSpace(appium.Udid))
        {
            options.AddAdditionalAppiumOption("udid", appium.Udid);
        }

        options.AddAdditionalAppiumOption("noReset", appium.NoReset);
        options.AddAdditionalAppiumOption("fullReset", appium.FullReset);
        options.AddAdditionalAppiumOption("newCommandTimeout", appium.NewCommandTimeoutSec);

        foreach (var (key, value) in appium.AdditionalCapabilities)
        {
            options.AddAdditionalAppiumOption(key, value);
        }

        return options;
    }

    public async Task<SessionArtifacts> FinalizeAsync(bool testFailed, string testName)
    {
        var keepVideo = _settings.Video == CaptureMode.Always
            || (_settings.Video == CaptureMode.OnFailure && testFailed);

        string? screenshotBase64 = null;
        string? videoPath = null;

        if (testFailed && _settings.Screenshot.CaptureOnFailure)
        {
            screenshotBase64 = await CaptureScreenshotAsync(testName);
        }

        if (_recording)
        {
            videoPath = await StopRecordingAsync(testName, keepVideo);
            _recording = false;
        }

        await DisposeAsync();

        // Appium has no Playwright-style trace, so it is always null here.
        return new SessionArtifacts(screenshotBase64, null, videoPath);
    }

    public async Task<string> CaptureScreenshotAsync(string testName)
    {
        var screenshot = await Task.Run(() => ((ITakesScreenshot)Driver).GetScreenshot());
        var file = BuildArtifactPath("screenshots", testName, "png");
        await File.WriteAllBytesAsync(file, screenshot.AsByteArray);

        _log.Debug("Screenshot saved to {File}", file);
        return screenshot.AsBase64EncodedString;
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
            if (Driver is not null)
            {
                await Task.Run(() => Driver.Quit());
                Driver.Dispose();
            }
        }
        catch (Exception ex)
        {
            _log.Warning("Error while disposing Appium session: {Message}", ex.Message);
        }
    }

    private void TryStartRecording()
    {
        try
        {
            Driver.StartRecordingScreen();
            _recording = true;
            _log.Debug("Screen recording started");
        }
        catch (Exception ex)
        {
            _log.Warning("Could not start screen recording ({Message}). Continuing without video.", ex.Message);
        }
    }

    private async Task<string?> StopRecordingAsync(string testName, bool keepVideo)
    {
        try
        {
            var base64 = await Task.Run(() => Driver.StopRecordingScreen());
            if (!keepVideo || string.IsNullOrEmpty(base64))
            {
                return null;
            }

            var videoPath = BuildArtifactPath("videos", testName, "mp4");
            await File.WriteAllBytesAsync(videoPath, Convert.FromBase64String(base64));
            _log.Information("Video saved: {Path}", videoPath);
            return videoPath;
        }
        catch (Exception ex)
        {
            _log.Warning("Could not save screen recording: {Message}", ex.Message);
            return null;
        }
    }

    private string ResolveAppPath(string app)
    {
        if (string.IsNullOrWhiteSpace(app) || app.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return app;
        }

        if (Path.IsPathRooted(app))
        {
            return app;
        }

        // Relative paths resolve against the test output folder (where the .apk is typically copied).
        var candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, app));
        if (File.Exists(candidate))
        {
            return candidate;
        }

        _log.Warning("App path '{App}' was not found relative to '{Base}'; passing it to Appium as-is.",
            app, AppContext.BaseDirectory);
        return candidate;
    }

    private static string BuildArtifactPath(string subFolder, string testName, string extension)
    {
        var dir = Path.Combine(FrameworkConfig.ResultsDirectory, subFolder);
        Directory.CreateDirectory(dir);

        var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(dir, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.{extension}");
    }
}
