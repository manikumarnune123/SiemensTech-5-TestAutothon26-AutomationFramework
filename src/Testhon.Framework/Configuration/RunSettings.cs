using Testhon.Framework.Enums;

namespace Testhon.Framework.Configuration;

/// <summary>
/// Strongly-typed run settings bound from <c>appsettings*.json</c> (section "RunSettings")
/// and overridable via environment variables (e.g. <c>RunSettings__Headless=false</c>).
/// </summary>
public sealed class RunSettings
{
    public BrowserEngine Browser { get; set; } = BrowserEngine.Chromium;
    public bool Headless { get; set; } = true;
    public float SlowMo { get; set; }
    public int DefaultTimeoutMs { get; set; } = 30_000;
    public int NavigationTimeoutMs { get; set; } = 60_000;

    /// <summary>Desktop or Android (device emulation).</summary>
    public Platform Platform { get; set; } = Platform.Desktop;

    /// <summary>Playwright device descriptor used when <see cref="Platform"/> is Android (e.g. "Pixel 5").</summary>
    public string DeviceName { get; set; } = "Pixel 5";

    /// <summary>Chrome DevTools endpoint for a real Android device (used when Platform is RealAndroid).</summary>
    public string AndroidCdpEndpoint { get; set; } = "http://localhost:9222";

    /// <summary>Run 'adb forward' automatically before connecting to a real device.</summary>
    public bool AutoAdbForward { get; set; } = true;

    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Number of extra attempts for flaky element actions.</summary>
    public int RetryCount { get; set; } = 2;

    /// <summary>Playwright trace capture policy (viewable with <c>playwright show-trace</c>).</summary>
    public CaptureMode Trace { get; set; } = CaptureMode.OnFailure;

    /// <summary>Video capture policy for the session.</summary>
    public CaptureMode Video { get; set; } = CaptureMode.OnFailure;

    public ViewportSettings Viewport { get; set; } = new();
    public ScreenshotSettings Screenshot { get; set; } = new();

    /// <summary>Appium settings for native / mobile-web automation (used by the mobile test stack).</summary>
    public AppiumSettings Appium { get; set; } = new();
}

public sealed class ViewportSettings
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
}

public sealed class ScreenshotSettings
{
    public bool CaptureOnFailure { get; set; } = true;
    public bool FullPage { get; set; } = true;
}

/// <summary>
/// Appium capabilities and connection settings. Bind from the "RunSettings:Appium" section or
/// override via environment variables (e.g. <c>RunSettings__Appium__App=C:\build\app.apk</c>).
/// </summary>
public sealed class AppiumSettings
{
    /// <summary>Appium server endpoint (the running <c>appium</c> process).</summary>
    public string ServerUrl { get; set; } = "http://127.0.0.1:4723";

    /// <summary>"Android" or "iOS".</summary>
    public string PlatformName { get; set; } = "Android";

    /// <summary>Automation engine (e.g. "UiAutomator2" for Android, "XCUITest" for iOS).</summary>
    public string AutomationName { get; set; } = "UiAutomator2";

    /// <summary>Target device / emulator name (or "Android Emulator").</summary>
    public string DeviceName { get; set; } = "Android Emulator";

    /// <summary>OS version of the target device (optional; helps device selection).</summary>
    public string PlatformVersion { get; set; } = string.Empty;

    /// <summary>Specific device id (from <c>adb devices</c>); optional.</summary>
    public string Udid { get; set; } = string.Empty;

    /// <summary>Path or URL to the .apk / .ipa to install and launch (native app testing).</summary>
    public string App { get; set; } = string.Empty;

    /// <summary>App package to launch when the app is already installed (native; alternative to <see cref="App"/>).</summary>
    public string AppPackage { get; set; } = string.Empty;

    /// <summary>App activity to launch (Android native; used with <see cref="AppPackage"/>).</summary>
    public string AppActivity { get; set; } = string.Empty;

    /// <summary>Mobile browser to drive for mobile-web testing (e.g. "Chrome"). Leave empty for native apps.</summary>
    public string BrowserName { get; set; } = string.Empty;

    /// <summary>Base URL opened in the mobile browser when <see cref="BrowserName"/> is set.</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Keep app data/state between sessions (faster, but less isolated).</summary>
    public bool NoReset { get; set; } = true;

    /// <summary>Fully reinstall/clear the app before the session.</summary>
    public bool FullReset { get; set; }

    /// <summary>Auto-accept runtime permission dialogs (Android).</summary>
    public bool AutoGrantPermissions { get; set; } = true;

    /// <summary>Seconds Appium waits between commands before ending the session.</summary>
    public int NewCommandTimeoutSec { get; set; } = 120;

    /// <summary>Overall command timeout for the WebDriver connection, in seconds.</summary>
    public int CommandTimeoutSec { get; set; } = 180;

    /// <summary>Any extra Appium capabilities not covered above (verbatim key/value pairs).</summary>
    public Dictionary<string, string> AdditionalCapabilities { get; set; } = new();
}
