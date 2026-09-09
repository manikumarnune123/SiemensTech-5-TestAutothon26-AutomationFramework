using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Enums;

namespace Testhon.Framework.Elements;

/// <summary>
/// Central place for mobile element handling. All mobile page objects go through this wrapper
/// instead of touching the Appium <see cref="AppiumDriver"/> directly, keeping waiting, logging
/// and retry behavior consistent — the mobile counterpart of <see cref="ElementActions"/>.
/// The Appium client is synchronous, so blocking calls are marshalled onto the thread pool to
/// preserve the framework's async surface.
/// </summary>
public sealed class AppiumElementActions : IMobileElementActions
{
    private readonly AppiumDriver _driver;
    private readonly ILogger _log;
    private readonly int _retryCount;
    private readonly TimeSpan _timeout;
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    public AppiumElementActions(AppiumDriver driver, ILogger log, RunSettings settings)
    {
        _driver = driver;
        _log = log;
        _retryCount = Math.Max(0, settings.RetryCount);
        _timeout = TimeSpan.FromMilliseconds(settings.DefaultTimeoutMs);
    }

    public Task TapAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        return RunStepAsync(() => WaitForElement(locator).Click(), $"Tap '{name}'");
    }

    public Task TypeAsync(MobileLocator locator, string text, string? description = null, bool mask = false)
    {
        var name = description ?? locator.ToString();
        var display = mask ? "******" : text;
        return RunStepAsync(() =>
        {
            var element = WaitForElement(locator);
            element.Clear();
            element.SendKeys(text);
        }, $"Type '{display}' into '{name}'");
    }

    public Task ClearAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        return RunStepAsync(() => WaitForElement(locator).Clear(), $"Clear '{name}'");
    }

    public async Task<string> GetTextAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        var text = await RunStepAsync(() => (WaitForElement(locator).Text ?? string.Empty).Trim(), $"Get text from '{name}'");
        _log.Information("Text of '{Name}' = '{Text}'", name, text);
        return text;
    }

    public async Task<string> GetAttributeAsync(MobileLocator locator, string attribute, string? description = null)
    {
        var name = description ?? locator.ToString();
        var value = await RunStepAsync(
            () => WaitForElement(locator).GetAttribute(attribute) ?? string.Empty,
            $"Get attribute '{attribute}' from '{name}'");
        _log.Information("Attribute '{Attribute}' of '{Name}' = '{Value}'", attribute, name, value);
        return value;
    }

    public Task<bool> IsDisplayedAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        return Task.Run(() =>
        {
            try
            {
                var displayed = _driver.FindElement(locator.ToBy()).Displayed;
                _log.Debug("Visibility of '{Name}' = {Displayed}", name, displayed);
                return displayed;
            }
            catch (WebDriverException ex)
            {
                _log.Debug("Visibility check for '{Name}' returned false: {Message}", name, ex.Message);
                return false;
            }
        });
    }

    public Task WaitForVisibleAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        _log.Information("Step: Wait for '{Name}' to be visible", name);
        return Task.Run(() => WaitForElement(locator));
    }

    public Task WaitForHiddenAsync(MobileLocator locator, string? description = null)
    {
        var name = description ?? locator.ToString();
        _log.Information("Step: Wait for '{Name}' to be hidden", name);
        return Task.Run(() =>
        {
            var deadline = DateTime.UtcNow + _timeout;
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    if (!_driver.FindElement(locator.ToBy()).Displayed)
                    {
                        return;
                    }
                }
                catch (WebDriverException)
                {
                    return; // gone from the tree = hidden
                }
                Thread.Sleep(PollInterval);
            }
            throw new WebDriverTimeoutException($"Element {locator} still visible after {_timeout.TotalSeconds:0}s.");
        });
    }

    public async Task<int> CountAsync(MobileLocator locator, string? description = null)
    {
        var count = await Task.Run(() => _driver.FindElements(locator.ToBy()).Count);
        _log.Debug("Count of '{Name}' = {Count}", description ?? locator.ToString(), count);
        return count;
    }

    public Task SwipeAsync(SwipeDirection direction, double fraction = 0.75, string? description = null)
    {
        var name = description ?? direction.ToString();
        return RunStepAsync(() =>
        {
            var size = _driver.Manage().Window.Size;
            var args = new Dictionary<string, object>
            {
                ["left"] = (int)(size.Width * 0.1),
                ["top"] = (int)(size.Height * 0.1),
                ["width"] = (int)(size.Width * 0.8),
                ["height"] = (int)(size.Height * 0.8),
                ["direction"] = direction.ToString().ToLowerInvariant(),
                ["percent"] = Math.Clamp(fraction, 0.1, 1.0)
            };
            _driver.ExecuteScript("mobile: swipeGesture", args);
        }, $"Swipe {name}");
    }

    public Task ScrollToTextAsync(string text, string? description = null)
    {
        var name = description ?? text;
        var expression =
            "new UiScrollable(new UiSelector().scrollable(true))" +
            $".scrollIntoView(new UiSelector().textContains(\"{text}\"))";
        return RunStepAsync(
            () => _driver.FindElement(MobileBy.AndroidUIAutomator(expression)),
            $"Scroll to text '{name}'");
    }

    public Task HideKeyboardAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                if (_driver is AndroidDriver android && android.IsKeyboardShown())
                {
                    android.HideKeyboard();
                    _log.Debug("Keyboard hidden");
                }
            }
            catch (WebDriverException ex)
            {
                _log.Debug("HideKeyboard skipped: {Message}", ex.Message);
            }
        });
    }

    // Polls until the element exists and is displayed, or the timeout elapses.
    private IWebElement WaitForElement(MobileLocator locator)
    {
        var by = locator.ToBy();
        var deadline = DateTime.UtcNow + _timeout;
        WebDriverException? last = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var element = _driver.FindElement(by);
                if (element.Displayed)
                {
                    return element;
                }
            }
            catch (WebDriverException ex)
            {
                last = ex;
            }
            Thread.Sleep(PollInterval);
        }

        throw new WebDriverTimeoutException(
            $"Element {locator} was not visible after {_timeout.TotalSeconds:0}s.", last);
    }

    private async Task RunStepAsync(Action action, string stepName)
    {
        _log.Information("Step: {Step}", stepName);
        var totalAttempts = _retryCount + 1;

        for (var attempt = 1; attempt <= totalAttempts; attempt++)
        {
            try
            {
                await Task.Run(action);
                return;
            }
            catch (WebDriverException ex) when (attempt < totalAttempts)
            {
                _log.Warning("Attempt {Attempt}/{Total} failed for '{Step}': {Message}. Retrying...",
                    attempt, totalAttempts, stepName, ex.Message);
                await Task.Delay(500);
            }
        }
    }

    private async Task<T> RunStepAsync<T>(Func<T> func, string stepName)
    {
        _log.Information("Step: {Step}", stepName);
        var totalAttempts = _retryCount + 1;
        Exception? last = null;

        for (var attempt = 1; attempt <= totalAttempts; attempt++)
        {
            try
            {
                return await Task.Run(func);
            }
            catch (WebDriverException ex) when (attempt < totalAttempts)
            {
                last = ex;
                _log.Warning("Attempt {Attempt}/{Total} failed for '{Step}': {Message}. Retrying...",
                    attempt, totalAttempts, stepName, ex.Message);
                await Task.Delay(500);
            }
        }

        throw last ?? new InvalidOperationException($"Step '{stepName}' failed without an exception.");
    }
}
