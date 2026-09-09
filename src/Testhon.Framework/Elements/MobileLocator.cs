using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Testhon.Framework.Elements;

/// <summary>
/// A technology-neutral mobile element locator. Wraps the common Appium locator strategies so
/// page objects express intent (accessibility id, resource id, xpath, ...) without depending on
/// Appium types directly.
/// </summary>
public readonly record struct MobileLocator(string Strategy, string Value)
{
    /// <summary>Matches by accessibility id (Android content-desc / iOS accessibility identifier).</summary>
    public static MobileLocator AccessibilityId(string value) => new("accessibility id", value);

    /// <summary>Matches by native id (Android resource-id, e.g. "com.app:id/username").</summary>
    public static MobileLocator Id(string value) => new("id", value);

    /// <summary>Matches by XPath (portable but slower).</summary>
    public static MobileLocator Xpath(string value) => new("xpath", value);

    /// <summary>Matches by native class name (e.g. "android.widget.EditText").</summary>
    public static MobileLocator ClassName(string value) => new("class name", value);

    /// <summary>Matches by name attribute.</summary>
    public static MobileLocator Name(string value) => new("name", value);

    /// <summary>Matches with an Android UiAutomator selector expression.</summary>
    public static MobileLocator AndroidUiAutomator(string value) => new("-android uiautomator", value);

    /// <summary>Matches with an iOS NSPredicate string.</summary>
    public static MobileLocator IosPredicate(string value) => new("-ios predicate string", value);

    /// <summary>Converts to the Appium <see cref="By"/> used by the WebDriver session.</summary>
    public By ToBy() => Strategy switch
    {
        "accessibility id" => MobileBy.AccessibilityId(Value),
        "id" => By.Id(Value),
        "xpath" => By.XPath(Value),
        "class name" => By.ClassName(Value),
        "name" => By.Name(Value),
        "-android uiautomator" => MobileBy.AndroidUIAutomator(Value),
        "-ios predicate string" => new ByIosNSPredicate(Value),
        _ => throw new NotSupportedException($"Unsupported mobile locator strategy '{Strategy}'.")
    };

    public override string ToString() => $"{Strategy}='{Value}'";
}
