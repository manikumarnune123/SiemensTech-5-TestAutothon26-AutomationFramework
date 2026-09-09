using NUnit.Framework;
using Testhon.Framework.Configuration;
using Testhon.Tests.Base;
using Testhon.Tests.Pages.Mobile;
using Testhon.Tests.Support;

namespace Testhon.Tests.Tests;

/// <summary>
/// Sample Appium mobile tests for the Gajab APK. Marked <see cref="ExplicitAttribute"/> because they
/// need a running Appium server plus a device/emulator with the app installed, so they are skipped by
/// default runs (including CI web runs). Run them explicitly:
/// <code>dotnet test --filter "TestCategory=Mobile"</code>
/// Configure the server, device and APK path under "RunSettings:Appium" in appsettings.json (or via
/// RunSettings__Appium__* environment variables).
/// </summary>
[TestFixture]
[Explicit("Requires a running Appium server and a device/emulator with the Gajab app installed.")]
[Category("Mobile")]
public sealed class MobileLoginTests : MobileBaseTest
{
    private static readonly LoginUsers Users = TestDataProvider.Load<LoginUsers>("TestData/users.json");

    [Test]
    public async Task ValidLogin_ShouldLoadHome()
    {
        var login = new MobileLoginPage(Driver, Mobile, Log, Settings);

        var home = await login.LoginAsync(Users.StandardUser.Username, Users.StandardUser.Password);

        await home.AssertLoadedAsync();
        Assert.That(await home.IsLoadedAsync(), Is.True);
    }

    [Test]
    public async Task InvalidLogin_ShouldShowError()
    {
        var login = new MobileLoginPage(Driver, Mobile, Log, Settings);

        await login.LoginAsync(Users.LockedOutUser.Username, Users.LockedOutUser.Password);

        Assert.That(await login.IsErrorVisibleAsync(), Is.True);
    }
}
