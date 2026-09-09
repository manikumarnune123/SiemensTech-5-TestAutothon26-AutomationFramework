using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Enums;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages.Gajab;

/// <summary>Page object for the Gajab storefront home page (https://gajab.com/).</summary>
public sealed class GajabHomePage : BasePage
{
    // ":visible" disambiguates the desktop nav from the hidden mobile-nav duplicate of the same control.
    private const string LoginSignupLink = "#header-login-btn";
    private const string HomeRoot = "#home-page-root";
    private const string HomeLogo = "#header-logo-link";
    private const string LocationButton = "#location-desktop-menu-btn";
    private const string LocationText = "#location-desktop-menu-text";
    private const string JustBargainedTitle = "#home-wp1-title";
    private const string JustBargainedViewMore = "#home-wp1-view-more";
    private const string LiveOrdersSection = "#home-live-orders-section";
    private const string LiveOrderCard = "#home-live-orders-container [id^='home-live-orders-card-']";
    private const string LiveOrderName = "#home-live-orders-container [id^='home-live-orders-name-']";
    private const string LiveOrderInfo = "#home-live-orders-container [id^='home-live-orders-info-']";
    private const string ProfileMenuButton = "#profile-menu-desktop-btn:visible";

    public GajabHomePage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
        : base(page, elements, log, settings)
    {
    }

    /// <summary>
    /// Blocks known-flaky third-party hosts (analytics, maps) that otherwise stall hydration on
    /// this staging environment, then navigates to the home page.
    /// </summary>
    public async Task NavigateWithNetworkGuardAsync()
    {
        await NavigateAsync(waitUntil: WaitUntilState.DOMContentLoaded);
        await MaximizeWindowAsync();
    }

    public async Task AssertChallengeWidgetsLoadedAsync()
    {
        await Elements.ExpectVisibleAsync(HomeRoot, "Gajab home page");
        await Elements.ExpectVisibleAsync(HomeLogo, "Gajab logo");
        await Elements.ExpectVisibleAsync(JustBargainedTitle, "Just Bargained section");
        await Elements.ExpectVisibleAsync(LiveOrdersSection, "Live Orders section");
    }

    public async Task<string> GetSelectedLocationAsync()
    {
        await Elements.ExpectVisibleAsync(LocationText, "Selected location");
        return await Elements.GetTextAsync(LocationText, "Selected location");
    }

    public async Task SelectLocationAsync(string location)
    {
        await Elements.ClickAsync(LocationButton, "Location menu");
        await Elements.ExpectContainsTextAsync(LocationText, location, "Selected location");
    }

    public async Task<(string Name, string Details)> GetLatestLiveOrderAsync(string screenshotPath)
    {
        await Elements.ExpectVisibleAsync(LiveOrderCard, "Latest live order");
        await Page.Locator(LiveOrderCard).First.ScreenshotAsync(new LocatorScreenshotOptions
        {
            Path = screenshotPath
        });

        var name = await Elements.GetTextAsync(LiveOrderName, "Live order customer");
        var details = await Elements.GetTextAsync(LiveOrderInfo, "Live order details");
        return (name, details);
    }

    public async Task OpenJustBargainedAsync()
    {
        await Elements.ClickAsync(JustBargainedViewMore, "Just Bargained View All");
        await Page.WaitForURLAsync(url => url.Contains("/product-list/all", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = Settings.NavigationTimeoutMs });
    }

    // --start-maximized only sizes the OS window at launch; the CDP call re-asserts it once the
    // page has actually opened. CDP is Chromium-only, so this is skipped for other engines/headless.
    private async Task MaximizeWindowAsync()
    {
        if (Settings.Headless || Settings.Browser != BrowserEngine.Chromium)
        {
            return;
        }

        try
        {
            var session = await Page.Context.NewCDPSessionAsync(Page);
            var window = await session.SendAsync("Browser.getWindowForTarget");
            if (window is not { } windowResult)
            {
                return;
            }

            var windowId = windowResult.GetProperty("windowId").GetInt32();
            await session.SendAsync("Browser.setWindowBounds", new Dictionary<string, object>
            {
                ["windowId"] = windowId,
                ["bounds"] = new Dictionary<string, object> { ["windowState"] = "maximized" }
            });
        }
        catch (PlaywrightException ex)
        {
            Log.Debug(ex, "Could not maximize the browser window via CDP.");
        }
    }

    public async Task<GajabLoginPage> OpenLoginAsync()
    {
        await Elements.ClickAsync(LoginSignupLink, "Log in / Sign up button");
        return new GajabLoginPage(Page, Elements, Log, Settings);
    }

    /// <summary>True once the header shows the authenticated profile menu instead of the sign-in link.</summary>
    public Task<bool> IsLoggedInAsync() => Elements.IsVisibleAsync(ProfileMenuButton, "Logged-in profile menu");
}

