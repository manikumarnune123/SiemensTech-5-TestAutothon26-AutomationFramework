using NUnit.Framework;
using Testhon.Framework.Configuration;
using Testhon.Tests.Base;
using Testhon.Tests.Pages.Gajab;
using Testhon.Tests.Support;

namespace Testhon.Tests.Tests;

[TestFixture]
public sealed class GajabChallengeTests : BaseTest
{
    private static readonly GajabUsers Users = TestDataProvider.Load<GajabUsers>("TestData/gajab_users.json");

    [Test]
    [Category("Challenge")]
    [Category("Gajab")]
    [Explicit("Runs the complete staging-site TestAutothon workflow and may place a test order.")]
    public async Task CompleteCustomerJourney_ShouldPlaceOrderAndShowSavings()
    {
        var home = new GajabHomePage(Page, Elements, Log, Settings);
        await home.NavigateWithNetworkGuardAsync();
        var login = await home.OpenLoginAsync();
        var loggedInHome = await login.LoginWithOtpAsync(Users.StandardUser.MobileNumber, Users.StandardUser.Otp);

        var challenge = new GajabChallengePage(Page, Elements, Log, Settings);
        await challenge.SetLocationAsync("560037");
        await challenge.CaptureDealOfTheDayAsync(Path.Combine(TestContext.CurrentContext.WorkDirectory, "deal-of-day.png"));
        await challenge.GetMostBargainedTrendingProductAsync();
        await challenge.CaptureLatestLiveOrderAsync(Path.Combine(TestContext.CurrentContext.WorkDirectory, "latest-live-order.png"));
        await challenge.OpenToysAndGamesAsync();
        await challenge.SelectBrandAndPriceRangeAsync(427, 727);
        await challenge.OpenDartboardProductAsync();
        await challenge.BargainThreeTimesAndAcceptAsync();
        await challenge.ProceedToPaymentAsync();
        await challenge.VerifySavingsAsync();

        Assert.That(await loggedInHome.IsLoggedInAsync(), Is.True, "The session should remain authenticated.");
    }
}