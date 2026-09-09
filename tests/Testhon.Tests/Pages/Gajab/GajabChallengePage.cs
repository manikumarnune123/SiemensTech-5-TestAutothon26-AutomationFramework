using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages.Gajab;

public sealed class GajabChallengePage : BasePage
{
    private const string LocationInput = "#location-search-input, input[placeholder*='Pincode'], input[placeholder*='City']";
    private const string LocationSuggestion = "[role='option'], text=/Central Delhi|Bengaluru|Mumbai/i";
    private const string DealSection = "#home-wp1-most-bargained-card, #home-bargained-products-section";
    private const string DealTitle = "#home-wp1-product-title";
    private const string DealPrice = "#home-wp1-asking-price-p";
    private const string DealImage = "#home-wp1-product-img";
    private const string TrendingSection = "#home-wp2-trending-card";
    private const string TrendingCards = "#home-wp2-items-grid > [id^='home-wp2-item-card-']";
    private const string TrendingBargainCounts = "[id^='home-wp2-item-bargain-count-']";
    private const string ToysGamesLink = "#category-list-desktop-link-17, a[href*='/product-list/toys-games/17']";
    private const string MyBargainsLink = "#header-my-bargains-btn";
    private const string ProductList = "#product-list, [data-testid='product-list'], main";
    private const string BrandFilter = "#brand-filter, [id*='brand']";
    private const string PriceFilter = "#price-filter, [id*='price']";
    private const string ProductTitle = "text='Classic 15.7 Inch Soft Tip Dartboard Game Set'";
    private const string BargainDialog = "[role='dialog'], text='Bargain with Seller'";
    private const string OfferButton = "#offer-price-btn, button:has-text('Offer Your Price')";
    private const string AcceptButton = "#accept-offer-btn, button:has-text('Accept the offer')";
    private const string BuyNowButton = "#buy-now-btn, button:has-text('Buy Now')";
    private const string PayOnline = "#pay-online, label:has-text('Pay Online'), text='Pay Online'";
    private const string PayButton = "#pay-btn, button:has-text('Pay')";
    private const string NetBanking = "#netbanking, text='Netbanking'";
    private const string PaymentSuccess = "#success, button:has-text('Success')";
    private const string OrderPlaced = "text='Order placed!', text='Your order has been placed'";
    private const string Savings = "text=/saved|savings/i";

    public GajabChallengePage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
        : base(page, elements, log, settings)
    {
    }

    public async Task SetLocationAsync(string location)
    {
        await Elements.ClickAsync("#location-desktop-menu-btn", "Location menu");
        await Elements.FillAsync(LocationInput, location, "Pincode or city", mask: true);
        await Elements.ClickAsync(LocationSuggestion, "Location suggestion");
        await Elements.ExpectContainsTextAsync("#location-desktop-menu-text, #location-mobile-location-text", location,
            "Selected location");
    }

    public async Task<(string Name, string Price, string Image)> CaptureDealOfTheDayAsync(string screenshotPath)
    {
        await Elements.ExpectVisibleAsync(DealSection, "Deal of the Day section");
        await Page.Locator(DealSection).ScreenshotAsync(new LocatorScreenshotOptions { Path = screenshotPath });
        return (
            await Elements.GetTextAsync(DealTitle, "Deal product name"),
            await Elements.GetTextAsync(DealPrice, "Deal asking price"),
            await Page.Locator(DealImage).GetAttributeAsync("src") ?? string.Empty);
    }

    public async Task<(string Name, string Bargains)> GetMostBargainedTrendingProductAsync()
    {
        await Elements.ExpectVisibleAsync(TrendingSection, "Trending Products section");
        var cards = Page.Locator(TrendingCards);
        var count = await cards.CountAsync();
        AssertAtLeastOne(count, "Trending product");

        var bestIndex = 0;
        var bestBargains = -1;
        for (var index = 0; index < count; index++)
        {
            var text = await cards.Nth(index).Locator(TrendingBargainCounts).InnerTextAsync();
            var digits = new string(text.Where(char.IsDigit).ToArray());
            var bargains = int.TryParse(digits, out var parsed) ? parsed : 0;
            if (bargains > bestBargains)
            {
                bestIndex = index;
                bestBargains = bargains;
            }
        }

        return (
            (await cards.Nth(bestIndex).InnerTextAsync()).Trim(),
            bestBargains.ToString());
    }

    public async Task<(string Name, string Details)> CaptureLatestLiveOrderAsync(string screenshotPath)
    {
        var liveOrder = Page.Locator("#home-live-orders-container [id^='home-live-orders-card-']").First;
        await liveOrder.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await liveOrder.ScreenshotAsync(new LocatorScreenshotOptions { Path = screenshotPath });
        return (
            await liveOrder.Locator("[id^='home-live-orders-name-']").InnerTextAsync(),
            await liveOrder.Locator("[id^='home-live-orders-info-']").InnerTextAsync());
    }

    public async Task OpenToysAndGamesAsync()
    {
        await Elements.ClickAsync(ToysGamesLink, "Toys & Games category");
        await Page.WaitForURLAsync(url => url.Contains("toys-games", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = Settings.NavigationTimeoutMs });
    }

    public async Task SelectBrandAndPriceRangeAsync(int minimum, int maximum)
    {
        await Elements.ClickAsync(BrandFilter, "Brand filter");
        await Elements.ClickAsync("label:has-text(\"SERA’S BASKET\"), label:has-text(\"SERA'S BASKET\"), text='SERA’S BASKET'",
            "SERA’S BASKET brand");

        await Elements.ClickAsync(PriceFilter, "Price filter");
        var sliders = Page.Locator("#price-filter input[type='range'], input[type='range']");
        var sliderCount = await sliders.CountAsync();
        if (sliderCount >= 2)
        {
            await sliders.Nth(0).FillAsync(minimum.ToString());
            await sliders.Nth(1).FillAsync(maximum.ToString());
        }

        await Page.WaitForTimeoutAsync(500);
    }

    public async Task OpenDartboardProductAsync()
    {
        await Elements.ClickAsync(ProductList, "Filtered product list");
        await Elements.ClickAsync(ProductTitle, "Classic dartboard product");
        await Page.WaitForURLAsync(url => url.Contains("product-detail", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = Settings.NavigationTimeoutMs });
    }

    public async Task BargainThreeTimesAndAcceptAsync()
    {
        await Elements.ClickAsync("#start-bargaining-btn, button:has-text('Start Bargaining')", "Start bargaining");
        await Elements.ExpectVisibleAsync(BargainDialog, "Bargain dialog");

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            await Elements.ClickAsync(OfferButton, $"Bargain attempt {attempt}");
            await Page.Locator("#offer-input, input[placeholder*='offer'], input[type='number']").First
                .FillAsync((300 + attempt * 10).ToString());
        }

        await Elements.ClickAsync(AcceptButton, "Accept bargain offer");
        await Elements.ExpectVisibleAsync(BuyNowButton, "Buy Now button");
    }

    public async Task ProceedToPaymentAsync()
    {
        await Elements.ClickAsync(BuyNowButton, "Buy Now");
        await Elements.ClickAsync(PayOnline, "Pay Online");
        await Elements.ClickAsync(PayButton, "Pay");
        await Elements.ClickAsync(NetBanking, "Netbanking");
        await Page.Locator("#bank-list button, [role='option'], text=/Bank of India|Canara Bank|Central Bank/i").First.ClickAsync();
        await Elements.ClickAsync(PaymentSuccess, "Payment success");
        await Elements.ExpectVisibleAsync(OrderPlaced, "Order placed confirmation");
    }

    public async Task VerifySavingsAsync()
    {
        await Elements.ClickAsync(MyBargainsLink, "My Bargains");
        await Page.WaitForURLAsync(url => url.Contains("bargain", StringComparison.OrdinalIgnoreCase),
            new PageWaitForURLOptions { Timeout = Settings.NavigationTimeoutMs });
        await Elements.ExpectVisibleAsync(Savings, "Bargain savings");
    }

    private static void AssertAtLeastOne(int count, string description)
    {
        if (count == 0)
        {
            throw new InvalidOperationException($"No {description} was rendered by the staging environment.");
        }
    }
}