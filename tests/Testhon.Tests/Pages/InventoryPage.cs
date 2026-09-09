using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages;

/// <summary>Page object for the SauceDemo inventory (products) page.</summary>
public sealed class InventoryPage : BasePage
{
    private const string PageTitle = ".title";
    private const string InventoryList = ".inventory_list";
    private const string CartBadge = ".shopping_cart_badge";

    private static string AddToCartButton(string itemId) => $"[data-test='add-to-cart-{itemId}']";

    public InventoryPage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
        : base(page, elements, log, settings)
    {
    }

    public Task<bool> IsLoadedAsync() => Elements.IsVisibleAsync(InventoryList, "Inventory list");

    /// <summary>Web-first assertion variant preferred inside tests.</summary>
    public Task AssertLoadedAsync() => Elements.ExpectVisibleAsync(InventoryList, "Inventory list");

    public Task<string> GetTitleAsync() => Elements.GetTextAsync(PageTitle, "Products title");

    public Task AddItemToCartAsync(string itemId) =>
        Elements.ClickAsync(AddToCartButton(itemId), $"Add '{itemId}' to cart");

    public async Task<string> GetCartCountAsync()
    {
        var hasItems = await Elements.IsVisibleAsync(CartBadge, "Cart badge");
        return hasItems ? await Elements.GetTextAsync(CartBadge, "Cart badge") : "0";
    }
}
