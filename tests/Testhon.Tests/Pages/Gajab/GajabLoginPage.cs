using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;
using Testhon.Framework.Pages;

namespace Testhon.Tests.Pages.Gajab;

/// <summary>Page object for the Gajab mobile-number + OTP sign-in flow (/auth/signin).</summary>
public sealed class GajabLoginPage : BasePage
{
    private const string MobileNumberInput = "#signin-mobile-input";
    private const string TermsCheckbox = "#signin-terms-checkbox";
    private const string RequestOtpButton = "#signin-submit-btn";
    private const string SubmitButton = "#otp-submit-btn";
    private const string SuccessToast = "text=successful";

    public GajabLoginPage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
        : base(page, elements, log, settings)
    {
    }

    private static string OtpDigitBox(int index) => $"#otp-input-{index}";

    /// <summary>Enters the mobile number, accepts the terms and requests an OTP.</summary>
    public async Task RequestOtpAsync(string mobileNumber)
    {
        await Elements.FillAsync(MobileNumberInput, mobileNumber, "Mobile number field");
        await Elements.ClickAsync(TermsCheckbox, "Terms & privacy checkbox");
        await Elements.ClickAsync(RequestOtpButton, "Request OTP button");
        await Elements.WaitForVisibleAsync(OtpDigitBox(0), "OTP digit 1");
    }

    /// <summary>Fills the 6-digit OTP boxes. The site auto-submits once all digits are entered.</summary>
    public async Task EnterOtpAsync(string otp)
    {
        for (var i = 0; i < otp.Length && i < 6; i++)
        {
            await Elements.FillAsync(OtpDigitBox(i), otp[i].ToString(), $"OTP digit {i + 1}");
        }

        await TryObserveSuccessToastAsync();
        await WaitForVerificationToCompleteAsync();
    }

    /// <summary>Full flow: request OTP, verify it and land back on the (now authenticated) home page.</summary>
    public async Task<GajabHomePage> LoginWithOtpAsync(string mobileNumber, string otp = "123456")
    {
        Log.Information("Logging in with mobile number ending in {Last4}", mobileNumber[^4..]);
        await RequestOtpAsync(mobileNumber);
        await EnterOtpAsync(otp);
        return new GajabHomePage(Page, Elements, Log, Settings);
    }

    // Best-effort: the "OTP verified / login successful" toast auto-dismisses quickly, so a miss here
    // isn't fatal - the caller still confirms success via the home page's authenticated header state.
    private async Task TryObserveSuccessToastAsync()
    {
        try
        {
            await Page.Locator(SuccessToast).WaitForAsync(new LocatorWaitForOptions { Timeout = 4000 });
            Log.Information("Step: Login successful message displayed");
        }
        catch (TimeoutException)
        {
            Log.Debug("Login successful toast was not observed (likely auto-dismissed before the check ran).");
        }
    }

    // The OTP form auto-submits once the 6th digit is entered; only click Submit if that didn't happen.
    private async Task WaitForVerificationToCompleteAsync()
    {
        try
        {
            await Page.WaitForURLAsync(url => !url.Contains("otp-verification"),
                new PageWaitForURLOptions { Timeout = 5000 });
        }
        catch (TimeoutException)
        {
            if (await Elements.IsVisibleAsync(SubmitButton, "Submit button"))
            {
                await Elements.ClickAsync(SubmitButton, "Submit button");
            }

            await Page.WaitForURLAsync(url => !url.Contains("otp-verification"),
                new PageWaitForURLOptions { Timeout = Settings.NavigationTimeoutMs });
        }
    }
}
