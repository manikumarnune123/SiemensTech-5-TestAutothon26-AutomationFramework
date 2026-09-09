using OpenQA.Selenium.Appium;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;

namespace Testhon.Framework.Pages;

/// <summary>
/// Base class for all mobile (Appium) page objects. Provides shared plumbing (driver, element
/// wrapper, logger, settings) so concrete screens express intent, not mechanics — the mobile
/// counterpart of <see cref="BasePage"/>.
/// </summary>
public abstract class MobilePage
{
    protected AppiumDriver Driver { get; }
    protected IMobileElementActions Elements { get; }
    protected ILogger Log { get; }
    protected RunSettings Settings { get; }

    protected MobilePage(AppiumDriver driver, IMobileElementActions elements, ILogger log, RunSettings settings)
    {
        Driver = driver;
        Elements = elements;
        Log = log;
        Settings = settings;
    }

    /// <summary>
    /// Navigates the mobile browser to <see cref="AppiumSettings.BaseUrl"/> plus an optional relative
    /// path. Only meaningful for mobile-web sessions (when a browser was configured).
    /// </summary>
    public async Task NavigateAsync(string relativePath = "")
    {
        var baseUrl = Settings.Appium.BaseUrl;
        var url = string.IsNullOrEmpty(relativePath)
            ? baseUrl
            : $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

        Log.Information("Navigating to {Url}", url);
        await Task.Run(() => Driver.Navigate().GoToUrl(url));
    }

    /// <summary>Reads the current activity/screen title where available.</summary>
    public Task<string> GetPageTitleAsync() => Task.Run(() => Driver.Title ?? string.Empty);
}
