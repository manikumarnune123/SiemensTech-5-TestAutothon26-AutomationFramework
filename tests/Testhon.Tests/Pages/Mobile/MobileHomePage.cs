using OpenQA.Selenium.Appium;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages.Mobile;

/// <summary>
/// Sample native-app home screen shown after a successful login. Locators are placeholders — update
/// them to match the real Gajab APK (inspect with Appium Inspector).
/// </summary>
public sealed class MobileHomePage : MobilePage
{
    private static readonly MobileLocator Header = MobileLocator.AccessibilityId("home-header");
    private static readonly MobileLocator AccountMenu = MobileLocator.AccessibilityId("account");

    public MobileHomePage(AppiumDriver driver, IMobileElementActions elements, ILogger log, RunSettings settings)
        : base(driver, elements, log, settings)
    {
    }

    public Task<bool> IsLoadedAsync() => Elements.IsDisplayedAsync(Header, "Home header");

    public Task AssertLoadedAsync() => Elements.WaitForVisibleAsync(Header, "Home header");

    public Task OpenAccountAsync() => Elements.TapAsync(AccountMenu, "Account menu");
}
