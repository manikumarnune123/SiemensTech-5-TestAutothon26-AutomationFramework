using NUnit.Framework;
using Testhon.Tests.Base;
using Testhon.Tests.Pages.Gajab;

namespace Testhon.Tests.Tests;

[TestFixture]
public sealed class GajabHomePageTests : BaseTest
{
    [Test]
    [Category("Smoke")]
    [Category("Gajab")]
    [Retry(2)]
    public async Task HomePage_ShouldDisplayChallengeWidgets()
    {
        var home = await OpenHomeAsync();

        await home.AssertChallengeWidgetsLoadedAsync();

        var location = await home.GetSelectedLocationAsync();
        Assert.That(location, Is.Not.Empty, "The home page should show the selected location.");
    }

    [Test]
    [Category("Regression")]
    [Category("Gajab")]
    [Retry(2)]
    public async Task HomePage_ShouldOpenJustBargainedProducts()
    {
        var home = await OpenHomeAsync();

        await home.OpenJustBargainedAsync();

        Assert.That(Page.Url, Does.Contain("/product-list/all"));
    }

    [Test]
    [Category("Regression")]
    [Category("Gajab")]
    [Retry(2)]
    public async Task HomePage_ShouldCaptureLatestLiveOrder()
    {
        var home = await OpenHomeAsync();
        var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "latest-live-order.png");

        var order = await home.GetLatestLiveOrderAsync(screenshotPath);

        Assert.Multiple(() =>
        {
            Assert.That(order.Name, Is.Not.Empty, "The latest live order should show a customer name.");
            Assert.That(order.Details, Is.Not.Empty, "The latest live order should show order details.");
            Assert.That(File.Exists(screenshotPath), Is.True, "A live-order screenshot should be captured.");
        });
    }

    private async Task<GajabHomePage> OpenHomeAsync()
    {
        var home = new GajabHomePage(Page, Elements, Log, Settings);
        await home.NavigateWithNetworkGuardAsync();
        return home;
    }
}