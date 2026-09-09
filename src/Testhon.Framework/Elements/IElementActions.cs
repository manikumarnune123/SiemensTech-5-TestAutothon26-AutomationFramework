namespace Testhon.Framework.Elements;

/// <summary>
/// Resilient, self-logging wrappers around common element interactions.
/// Every method logs a "Step:" entry (surfaced in logs and the HTML report) and applies
/// retry logic to smooth over transient flakiness.
/// </summary>
public interface IElementActions
{
    Task ClickAsync(string selector, string? description = null);

    /// <summary>Fills an input. Set <paramref name="mask"/> to keep secrets out of logs and reports.</summary>
    Task FillAsync(string selector, string value, string? description = null, bool mask = false);
    Task<string> GetTextAsync(string selector, string? description = null);
    Task<bool> IsVisibleAsync(string selector, string? description = null);
    Task WaitForVisibleAsync(string selector, string? description = null);
    Task ScrollIntoViewAsync(string selector, string? description = null);
    Task SelectDropdownAsync(string selector, string value, string? description = null);
    Task<int> CountAsync(string selector, string? description = null);

    /// <summary>Web-first assertion: auto-retries until the element is visible or times out.</summary>
    Task ExpectVisibleAsync(string selector, string? description = null);

    /// <summary>Web-first assertion: auto-retries until the element contains the expected text.</summary>
    Task ExpectContainsTextAsync(string selector, string expected, string? description = null);

    // ----- JavaScript execution (Selenium-style executeScript) -----

    /// <summary>Runs JavaScript in the page and returns a typed result. Pass <paramref name="arg"/> to expose data to the script.</summary>
    Task<T> ExecuteScriptAsync<T>(string script, object? arg = null);

    /// <summary>Runs JavaScript in the page for its side effect (no return value).</summary>
    Task ExecuteScriptAsync(string script, object? arg = null);

    /// <summary>Runs JavaScript with the matched element passed in as the first argument, returning a typed result.</summary>
    Task<T> ExecuteScriptOnAsync<T>(string selector, string script, string? description = null);

    /// <summary>Runs JavaScript with the matched element passed in as the first argument (no return value).</summary>
    Task ExecuteScriptOnAsync(string selector, string script, string? description = null);

    // ----- Shadow DOM -----
    // Open shadow roots are pierced automatically by the normal selector-based methods above.
    // The helpers below reach into a *closed* shadow root, which Playwright locators cannot.

    /// <summary>Reads text from an element inside a (possibly closed) shadow root: <c>host.shadowRoot.querySelector(inner)</c>.</summary>
    Task<string> GetTextInShadowAsync(string hostSelector, string innerSelector, string? description = null);

    /// <summary>Clicks an element inside a (possibly closed) shadow root: <c>host.shadowRoot.querySelector(inner)</c>.</summary>
    Task ClickInShadowAsync(string hostSelector, string innerSelector, string? description = null);

    // ----- Load / synchronization helpers -----

    /// <summary>Waits until the page reaches the <c>networkidle</c> load state (no in-flight requests).</summary>
    Task WaitForPageLoadAsync(string? description = null);

    /// <summary>Waits until the page fires <c>DOMContentLoaded</c>.</summary>
    Task WaitForDomContentLoadedAsync(string? description = null);

    /// <summary>Waits until a table/grid has rendered at least <paramref name="minimumRows"/> rows matching <paramref name="rowSelector"/>.</summary>
    Task WaitForTableToLoadAsync(string rowSelector, int minimumRows = 1, string? description = null);

    /// <summary>Waits until a custom JavaScript expression evaluates to truthy (polling with the default timeout).</summary>
    Task WaitForFunctionAsync(string jsExpression, object? arg = null, string? description = null);
}
