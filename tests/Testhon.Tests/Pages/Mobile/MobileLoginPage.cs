using OpenQA.Selenium.Appium;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages.Mobile;

/// <summary>
/// Sample native-app login screen for the Gajab APK.
///
/// The locators below are placeholders. Replace them with the real ones from the APK — inspect the
/// running app with <c>Appium Inspector</c> (or <c>uiautomatorviewer</c>) and prefer accessibility
/// ids / resource-ids over XPath. Resource-ids look like "com.gajab.app:id/username".
/// </summary>
public sealed class MobileLoginPage : MobilePage
{
    private static readonly MobileLocator UsernameField = MobileLocator.AccessibilityId("username");
    private static readonly MobileLocator PasswordField = MobileLocator.AccessibilityId("password");
    private static readonly MobileLocator LoginButton = MobileLocator.AccessibilityId("login");
    private static readonly MobileLocator ErrorBanner = MobileLocator.Id("com.gajab.app:id/error");

    public MobileLoginPage(AppiumDriver driver, IMobileElementActions elements, ILogger log, RunSettings settings)
        : base(driver, elements, log, settings)
    {
    }

    public async Task<MobileHomePage> LoginAsync(string username, string password)
    {
        Log.Information("Logging in as '{User}'", username);
        await Elements.TypeAsync(UsernameField, username, "Username field");
        await Elements.TypeAsync(PasswordField, password, "Password field", mask: true);
        await Elements.HideKeyboardAsync();
        await Elements.TapAsync(LoginButton, "Login button");
        return new MobileHomePage(Driver, Elements, Log, Settings);
    }

    public Task<bool> IsErrorVisibleAsync() => Elements.IsDisplayedAsync(ErrorBanner, "Error banner");

    public Task<string> GetErrorMessageAsync() => Elements.GetTextAsync(ErrorBanner, "Error banner");
}
