using NUnit.Framework;
using Testhon.Framework.Configuration;
using Testhon.Tests.Base;
using Testhon.Tests.Pages.Gajab;
using Testhon.Tests.Support;

namespace Testhon.Tests.Tests;

/// <summary>
/// Gajab (https://gajab.com/) desktop-web login flow: navigate to the site, open the
/// Log in / Sign up page, request an OTP for a mobile number and verify with the staging
/// environment's fixed OTP (123456 - no real SMS is sent).
/// Run against the "UAT" environment, which already points at the Gajab staging URL:
///   $env:TEST_ENV="UAT"; dotnet test --filter "FullyQualifiedName~GajabLoginTests"
/// </summary>
[TestFixture]
public sealed class GajabLoginTests : BaseTest
{
    private static readonly GajabUsers Users = TestDataProvider.Load<GajabUsers>("TestData/gajab_users.json");

    [Test]
    [Category("Smoke")]
    [Category("Gajab")]
    [Retry(2)]
    public async Task ValidMobileNumber_WithDefaultOtp_ShouldLoginSuccessfully()
    {
        var home = new GajabHomePage(Page, Elements, Log, Settings);
        // Blocks flaky analytics/maps hosts and doesn't wait for full "load" (the SPA never settles).
        await home.NavigateWithNetworkGuardAsync();

        Assert.That(await home.IsLoggedInAsync(), Is.False, "Test should start from a logged-out session.");

        var login = await home.OpenLoginAsync();
        var loggedInHome = await login.LoginWithOtpAsync(Users.StandardUser.MobileNumber, Users.StandardUser.Otp);

        Assert.That(await loggedInHome.IsLoggedInAsync(), Is.True,
            "Expected the header profile menu to indicate an authenticated session after OTP login.");
    }
}
