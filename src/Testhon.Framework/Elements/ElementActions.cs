using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using static Microsoft.Playwright.Assertions;

namespace Testhon.Framework.Elements;

/// <summary>
/// Central place for element handling. All page objects go through this wrapper instead of
/// touching <see cref="IPage"/> directly, which keeps waiting, logging, scrolling and retry
/// behavior consistent everywhere.
/// </summary>
public sealed class ElementActions : IElementActions
{
    private readonly IPage _page;
    private readonly ILogger _log;
    private readonly int _retryCount;
    private readonly float _timeout;
    private readonly float _navigationTimeout;

    public ElementActions(IPage page, ILogger log, RunSettings settings)
    {
        _page = page;
        _log = log;
        _retryCount = Math.Max(0, settings.RetryCount);
        _timeout = settings.DefaultTimeoutMs;
        _navigationTimeout = settings.NavigationTimeoutMs;
    }

    // Playwright locators are lazy by design: resolved on the action, with built-in auto-waiting.
    private ILocator Locate(string selector) => _page.Locator(selector);

    public Task ClickAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        return ExecuteWithRetryAsync(async () =>
        {
            var element = Locate(selector);
            await element.ScrollIntoViewIfNeededAsync();
            await element.ClickAsync(new LocatorClickOptions { Timeout = _timeout });
        }, $"Click '{name}'");
    }

    public Task FillAsync(string selector, string value, string? description = null, bool mask = false)
    {
        var name = description ?? selector;
        var display = mask ? "******" : value;
        return ExecuteWithRetryAsync(
            () => Locate(selector).FillAsync(value, new LocatorFillOptions { Timeout = _timeout }),
            $"Fill '{name}' with '{display}'");
    }

    public async Task<string> GetTextAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        var text = string.Empty;

        await ExecuteWithRetryAsync(async () =>
        {
            text = (await Locate(selector).InnerTextAsync(new LocatorInnerTextOptions { Timeout = _timeout })).Trim();
        }, $"Get text from '{name}'");

        _log.Information("Text of '{Name}' = '{Text}'", name, text);
        return text;
    }

    public async Task<bool> IsVisibleAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        try
        {
            var visible = await Locate(selector).IsVisibleAsync();
            _log.Debug("Visibility of '{Name}' = {Visible}", name, visible);
            return visible;
        }
        catch (PlaywrightException ex)
        {
            _log.Warning("Visibility check failed for '{Name}': {Message}", name, ex.Message);
            return false;
        }
    }

    public Task WaitForVisibleAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Wait for '{Name}' to be visible", name);
        return Locate(selector).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _timeout
        });
    }

    public Task ScrollIntoViewAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Scroll '{Name}' into view", name);
        return Locate(selector).ScrollIntoViewIfNeededAsync();
    }

    public Task SelectDropdownAsync(string selector, string value, string? description = null)
    {
        var name = description ?? selector;
        return ExecuteWithRetryAsync(
            () => Locate(selector).SelectOptionAsync(
                new[] { new SelectOptionValue { Value = value } },
                new LocatorSelectOptionOptions { Timeout = _timeout }),
            $"Select '{value}' in '{name}'");
    }

    public async Task<int> CountAsync(string selector, string? description = null)
    {
        var count = await Locate(selector).CountAsync();
        _log.Debug("Count of '{Name}' = {Count}", description ?? selector, count);
        return count;
    }

    public Task ExpectVisibleAsync(string selector, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Expect '{Name}' to be visible", name);
        return Expect(Locate(selector)).ToBeVisibleAsync();
    }

    public Task ExpectContainsTextAsync(string selector, string expected, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Expect '{Name}' to contain text '{Expected}'", name, expected);
        return Expect(Locate(selector)).ToContainTextAsync(expected);
    }

    // ---------- JavaScript execution ----------

    public async Task<T> ExecuteScriptAsync<T>(string script, object? arg = null)
    {
        _log.Information("Step: Execute script -> {Script}", Summarize(script));
        return await _page.EvaluateAsync<T>(script, arg);
    }

    public Task ExecuteScriptAsync(string script, object? arg = null)
    {
        _log.Information("Step: Execute script -> {Script}", Summarize(script));
        return _page.EvaluateAsync(script, arg);
    }

    public async Task<T> ExecuteScriptOnAsync<T>(string selector, string script, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Execute script on '{Name}' -> {Script}", name, Summarize(script));
        var result = default(T)!;
        await ExecuteWithRetryAsync(async () =>
        {
            result = await Locate(selector).EvaluateAsync<T>(script);
        }, $"Execute script on '{name}'");
        return result;
    }

    public Task ExecuteScriptOnAsync(string selector, string script, string? description = null)
    {
        var name = description ?? selector;
        _log.Information("Step: Execute script on '{Name}' -> {Script}", name, Summarize(script));
        return ExecuteWithRetryAsync(
            () => Locate(selector).EvaluateAsync(script),
            $"Execute script on '{name}'");
    }

    // ---------- Shadow DOM (closed roots) ----------

    public async Task<string> GetTextInShadowAsync(string hostSelector, string innerSelector, string? description = null)
    {
        var name = description ?? $"{hostSelector} >> shadow >> {innerSelector}";
        var text = string.Empty;

        await ExecuteWithRetryAsync(async () =>
        {
            text = (await _page.EvaluateAsync<string?>(
                @"([host, inner]) => {
                    const h = document.querySelector(host);
                    const root = h && h.shadowRoot;
                    if (!root) throw new Error('No shadow root on host: ' + host);
                    const el = root.querySelector(inner);
                    if (!el) throw new Error('Shadow element not found: ' + inner);
                    return (el.textContent || '').trim();
                }",
                new[] { hostSelector, innerSelector })) ?? string.Empty;
        }, $"Get text in shadow root '{name}'");

        _log.Information("Text of '{Name}' = '{Text}'", name, text);
        return text;
    }

    public Task ClickInShadowAsync(string hostSelector, string innerSelector, string? description = null)
    {
        var name = description ?? $"{hostSelector} >> shadow >> {innerSelector}";
        return ExecuteWithRetryAsync(
            () => _page.EvaluateAsync(
                @"([host, inner]) => {
                    const h = document.querySelector(host);
                    const root = h && h.shadowRoot;
                    if (!root) throw new Error('No shadow root on host: ' + host);
                    const el = root.querySelector(inner);
                    if (!el) throw new Error('Shadow element not found: ' + inner);
                    el.click();
                }",
                new[] { hostSelector, innerSelector }),
            $"Click in shadow root '{name}'");
    }

    // ---------- Load / synchronization ----------

    public Task WaitForPageLoadAsync(string? description = null)
    {
        _log.Information("Step: Wait for page load (networkidle){Extra}",
            description is null ? string.Empty : $" - {description}");
        return _page.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions { Timeout = _navigationTimeout });
    }

    public Task WaitForDomContentLoadedAsync(string? description = null)
    {
        _log.Information("Step: Wait for DOMContentLoaded{Extra}",
            description is null ? string.Empty : $" - {description}");
        return _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded,
            new PageWaitForLoadStateOptions { Timeout = _navigationTimeout });
    }

    public async Task WaitForTableToLoadAsync(string rowSelector, int minimumRows = 1, string? description = null)
    {
        var name = description ?? rowSelector;
        _log.Information("Step: Wait for table '{Name}' to load (>= {Min} row(s))", name, minimumRows);

        // First ensure at least one row is actually rendered and visible.
        await Locate(rowSelector).First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _timeout
        });

        // Then poll until the row count reaches the expected minimum.
        await _page.WaitForFunctionAsync(
            "([sel, min]) => document.querySelectorAll(sel).length >= min",
            new object[] { rowSelector, minimumRows },
            new PageWaitForFunctionOptions { Timeout = _timeout });

        var count = await Locate(rowSelector).CountAsync();
        _log.Information("Table '{Name}' loaded with {Count} row(s)", name, count);
    }

    public Task WaitForFunctionAsync(string jsExpression, object? arg = null, string? description = null)
    {
        _log.Information("Step: Wait for condition{Extra} -> {Script}",
            description is null ? string.Empty : $" '{description}'", Summarize(jsExpression));
        return _page.WaitForFunctionAsync(jsExpression, arg,
            new PageWaitForFunctionOptions { Timeout = _timeout });
    }

    private static string Summarize(string script)
    {
        var single = script.Replace("\r", " ").Replace("\n", " ").Trim();
        while (single.Contains("  "))
        {
            single = single.Replace("  ", " ");
        }
        return single.Length <= 80 ? single : single[..77] + "...";
    }

    private async Task ExecuteWithRetryAsync(Func<Task> action, string stepName)
    {
        _log.Information("Step: {Step}", stepName);
        var totalAttempts = _retryCount + 1;

        for (var attempt = 1; attempt <= totalAttempts; attempt++)
        {
            try
            {
                await action();
                return;
            }
            catch (PlaywrightException ex) when (attempt < totalAttempts)
            {
                _log.Warning("Attempt {Attempt}/{Total} failed for '{Step}': {Message}. Retrying...",
                    attempt, totalAttempts, stepName, ex.Message);
                await Task.Delay(500);
            }
        }
    }
}
