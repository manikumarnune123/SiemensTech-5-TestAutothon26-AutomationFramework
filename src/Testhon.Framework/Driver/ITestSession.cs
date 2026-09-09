namespace Testhon.Framework.Driver;

/// <summary>Diagnostic artifacts collected for a finished test.</summary>
public sealed record SessionArtifacts(string? ScreenshotBase64, string? TracePath, string? VideoPath);

/// <summary>
/// Common lifecycle for a single test's automation session, independent of the underlying
/// technology (Playwright for web, Appium for native/mobile-web). Lets the test base classes and
/// reporting pipeline treat every session uniformly.
/// </summary>
public interface ITestSession : IAsyncDisposable
{
    /// <summary>Starts the session (launches the browser/app and opens the first page/screen).</summary>
    Task InitializeAsync();

    /// <summary>
    /// Ends the session, collecting a failure screenshot plus trace/video according to the
    /// configured <see cref="Enums.CaptureMode"/>, and tears the session down.
    /// </summary>
    Task<SessionArtifacts> FinalizeAsync(bool testFailed, string testName);

    /// <summary>Captures a screenshot, persists it under the results folder and returns its Base64.</summary>
    Task<string> CaptureScreenshotAsync(string testName);
}
