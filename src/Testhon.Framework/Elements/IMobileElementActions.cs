using Testhon.Framework.Enums;

namespace Testhon.Framework.Elements;

/// <summary>
/// Resilient, self-logging wrappers around common mobile (Appium) interactions. Mirrors the
/// web <see cref="IElementActions"/> style: every method logs a "Step:" entry (surfaced in logs
/// and the HTML report) and applies wait/retry logic to smooth over transient flakiness.
/// </summary>
public interface IMobileElementActions
{
    /// <summary>Taps (clicks) the element, waiting for it to become available first.</summary>
    Task TapAsync(MobileLocator locator, string? description = null);

    /// <summary>Types text into an input. Set <paramref name="mask"/> to keep secrets out of logs/reports.</summary>
    Task TypeAsync(MobileLocator locator, string text, string? description = null, bool mask = false);

    /// <summary>Clears an input field.</summary>
    Task ClearAsync(MobileLocator locator, string? description = null);

    /// <summary>Reads the visible text of an element.</summary>
    Task<string> GetTextAsync(MobileLocator locator, string? description = null);

    /// <summary>Reads a native attribute (e.g. "text", "content-desc", "checked").</summary>
    Task<string> GetAttributeAsync(MobileLocator locator, string attribute, string? description = null);

    /// <summary>Returns whether the element is currently displayed (no throw if missing).</summary>
    Task<bool> IsDisplayedAsync(MobileLocator locator, string? description = null);

    /// <summary>Waits until the element is displayed or times out.</summary>
    Task WaitForVisibleAsync(MobileLocator locator, string? description = null);

    /// <summary>Waits until the element is gone from the screen or times out.</summary>
    Task WaitForHiddenAsync(MobileLocator locator, string? description = null);

    /// <summary>Counts elements matching the locator.</summary>
    Task<int> CountAsync(MobileLocator locator, string? description = null);

    /// <summary>Swipes across the screen in the given direction.</summary>
    Task SwipeAsync(SwipeDirection direction, double fraction = 0.75, string? description = null);

    /// <summary>Scrolls a scrollable container until an element with the given visible text appears (Android).</summary>
    Task ScrollToTextAsync(string text, string? description = null);

    /// <summary>Hides the on-screen keyboard if it is showing.</summary>
    Task HideKeyboardAsync();
}
