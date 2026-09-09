using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Enums;

namespace Testhon.Framework.Driver;

/// <summary>
/// Builds Playwright browsers and context options for both desktop and Android (device emulation).
/// Android emulation requires Chromium, so the engine is forced accordingly.
/// </summary>
public static class BrowserFactory
{
    public static async Task<IBrowser> LaunchAsync(IPlaywright playwright, RunSettings settings, ILogger log)
    {
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = settings.Headless,
            SlowMo = settings.SlowMo
        };

        var engine = settings.Platform == Platform.Android ? BrowserEngine.Chromium : settings.Browser;

        // Chromium-only flag: opens the actual OS window maximized instead of a fixed small window.
        if (!settings.Headless && settings.Platform != Platform.Android && engine == BrowserEngine.Chromium)
        {
            launchOptions.Args = new[] { "--start-maximized" };
        }

        log.Debug("Launching {Engine} (headless={Headless}, slowMo={SlowMo})",
            engine, settings.Headless, settings.SlowMo);

        return engine switch
        {
            BrowserEngine.Firefox => await playwright.Firefox.LaunchAsync(launchOptions),
            BrowserEngine.Webkit => await playwright.Webkit.LaunchAsync(launchOptions),
            _ => await playwright.Chromium.LaunchAsync(launchOptions)
        };
    }

    public static BrowserNewContextOptions BuildContextOptions(IPlaywright playwright, RunSettings settings)
    {
        BrowserNewContextOptions options;

        if (settings.Platform == Platform.Android)
        {
            if (!playwright.Devices.ContainsKey(settings.DeviceName))
            {
                throw new ArgumentException(
                    $"Unknown Playwright device descriptor '{settings.DeviceName}'. " +
                    "See https://playwright.dev/dotnet/docs/emulation for valid names.");
            }

            // Device descriptor sets viewport, user agent, deviceScaleFactor, isMobile and touch.
            options = playwright.Devices[settings.DeviceName];
        }
        else
        {
            // Null viewport lets the page fill the actual browser window instead of being
            // clipped to a fixed inner size.
            options = new BrowserNewContextOptions { ViewportSize = null };
        }

        if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            options.BaseURL = settings.BaseUrl;
        }

        options.IgnoreHTTPSErrors = true;
        return options;
    }
}
