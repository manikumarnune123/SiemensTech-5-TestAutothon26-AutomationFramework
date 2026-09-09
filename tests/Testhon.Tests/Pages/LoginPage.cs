using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages;

/// <summary>Page object for the SauceDemo login page (https://www.saucedemo.com).</summary>
public sealed class LoginPage : BasePage
{
    private const string UsernameInput = "#user-name";
    private const string PasswordInput = "#password";
    private const string LoginButton = "#login-button";
    private const string ErrorMessage = "[data-test='error']";

    public LoginPage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
        : base(page, elements, log, settings)
    {
    }

    public async Task<InventoryPage> LoginAsync(string username, string password)
    {
        Log.Information("Logging in as '{User}'", username);
        await Elements.FillAsync(UsernameInput, username, "Username field");
        await Elements.FillAsync(PasswordInput, password, "Password field", mask: true);
        await Elements.ClickAsync(LoginButton, "Login button");
        return new InventoryPage(Page, Elements, Log, Settings);
    }

    public async Task<string> GetErrorMessageAsync()
    {
        await Elements.WaitForVisibleAsync(ErrorMessage, "Error banner");
        return await Elements.GetTextAsync(ErrorMessage, "Error banner");
    }
}
