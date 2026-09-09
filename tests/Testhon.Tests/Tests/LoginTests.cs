using NUnit.Framework;
using Testhon.Framework.Configuration;
using Testhon.Tests.Base;
using Testhon.Tests.Pages;
using Testhon.Tests.Support;

namespace Testhon.Tests.Tests;

/// <summary>
/// Sample end-to-end tests demonstrating the framework. They run identically on desktop and
/// Android mobile web — only configuration changes.
/// </summary>
[TestFixture]
public sealed class LoginTests : BaseTest
{
    private static readonly LoginUsers Users = TestDataProvider.Load<LoginUsers>("TestData/users.json");

    [Test]
    [Category("Smoke")]
    [Retry(2)] // auto-reruns the whole test on assertion/timeout failure to absorb rare flakiness
    public async Task ValidLogin_ShouldLoadInventory()
    {
        var login = new LoginPage(Page, Elements, Log, Settings);
        await login.NavigateAsync();

        var inventory = await login.LoginAsync(Users.StandardUser.Username, Users.StandardUser.Password);

        await inventory.AssertLoadedAsync();
        Assert.That(await inventory.GetTitleAsync(), Is.EqualTo("Products"));
    }

    [Test]
    [Category("Regression")]
    public async Task LockedOutUser_ShouldSeeError()
    {
        var login = new LoginPage(Page, Elements, Log, Settings);
        await login.NavigateAsync();

        await login.LoginAsync(Users.LockedOutUser.Username, Users.LockedOutUser.Password);

        var error = await login.GetErrorMessageAsync();
        Assert.That(error, Does.Contain("locked out"));
    }

    [Test]
    [Category("Smoke")]
    public async Task AddItemToCart_ShouldUpdateBadge()
    {
        var login = new LoginPage(Page, Elements, Log, Settings);
        await login.NavigateAsync();

        var inventory = await login.LoginAsync(Users.StandardUser.Username, Users.StandardUser.Password);
        await inventory.AddItemToCartAsync("sauce-labs-backpack");

        Assert.That(await inventory.GetCartCountAsync(), Is.EqualTo("1"));
    }
}
