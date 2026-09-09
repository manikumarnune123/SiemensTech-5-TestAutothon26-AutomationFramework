using Microsoft.Playwright;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Elements;

namespace Testhon.Framework.Pages;

/// <summary>
/// Base class for all page objects. Provides shared plumbing (page, element wrapper, logger,
/// settings) and common navigation so concrete pages only express intent, not mechanics.
/// </summary>
public abstract class BasePage
{
    protected IPage Page { get; }
    protected IElementActions Elements { get; }
    protected ILogger Log { get; }
    protected RunSettings Settings { get; }

    protected BasePage(IPage page, IElementActions elements, ILogger log, RunSettings settings)
    {
        Page = page;
        Elements = elements;
        Log = log;
        Settings = settings;
    }

    /// <summary>Navigates to <see cref="RunSettings.BaseUrl"/> plus an optional relative path.</summary>
    public async Task NavigateAsync(string relativePath = "")
    {
        var url = string.IsNullOrEmpty(relativePath)
            ? Settings.BaseUrl
            : $"{Settings.BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

        Log.Information("Navigating to {Url}", url);
        await Page.GotoAsync(url);
    }

    public async Task<string> GetPageTitleAsync()
    {
        var title = await Page.TitleAsync();
        Log.Debug("Page title = {Title}", title);
        return title;
    }
}
