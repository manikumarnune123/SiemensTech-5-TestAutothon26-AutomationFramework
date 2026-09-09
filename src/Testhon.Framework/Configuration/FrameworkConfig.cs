using Microsoft.Extensions.Configuration;

namespace Testhon.Framework.Configuration;

/// <summary>
/// Centralized, environment-aware configuration provider.
/// Load order (later wins): appsettings.json → appsettings.{ENV}.json → environment variables.
/// The active environment is chosen by the <c>TEST_ENV</c> variable (defaults to "QA").
/// </summary>
public static class FrameworkConfig
{
    private static readonly Lazy<RunSettings> _settings = new(Load);

    /// <summary>Current active environment (QA, UAT, Prod, ...).</summary>
    public static string Environment =>
        System.Environment.GetEnvironmentVariable("TEST_ENV") ?? "QA";

    /// <summary>Singleton, thread-safe settings instance.</summary>
    public static RunSettings Settings => _settings.Value;

    /// <summary>
    /// Root folder for all run outputs (HTML reports, screenshots, traces, videos, logs). Uses the
    /// test project's conventional <c>TestResults</c> folder; override with <c>TESTHON_RESULTS_DIR</c>.
    /// </summary>
    public static string ResultsDirectory { get; } = ResolveResultsDirectory();

    private static string ResolveResultsDirectory()
    {
        var overrideDir = System.Environment.GetEnvironmentVariable("TESTHON_RESULTS_DIR");
        if (!string.IsNullOrWhiteSpace(overrideDir))
        {
            return Path.GetFullPath(overrideDir);
        }

        // Walk up from the output folder (bin/<cfg>/<tfm>) to the project root that owns the .csproj.
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (var i = 0; i < 6 && dir is not null; i++)
        {
            if (dir.EnumerateFiles("*.csproj").Any())
            {
                return Path.Combine(dir.FullName, "TestResults");
            }
            dir = dir.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, "TestResults");
    }

    private static RunSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var settings = new RunSettings();
        configuration.GetSection("RunSettings").Bind(settings);
        return settings;
    }
}
