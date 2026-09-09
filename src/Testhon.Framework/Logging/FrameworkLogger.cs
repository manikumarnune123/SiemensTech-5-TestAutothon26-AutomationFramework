using Serilog;
using Serilog.Events;
using Testhon.Framework.Configuration;
using Testhon.Framework.Reporting;

namespace Testhon.Framework.Logging;

/// <summary>
/// Configures the global Serilog logger. Writes to:
///  - Console (Information+) for live feedback,
///  - a rolling file (Debug+) under <c>logs/</c> for full traceability,
///  - the HTML report via <see cref="ExtentSink"/>.
/// </summary>
public static class FrameworkLogger
{
    public static void Initialize(RunSettings settings)
    {
        var logDir = Path.Combine(FrameworkConfig.ResultsDirectory, "logs");
        Directory.CreateDirectory(logDir);

        const string template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{TestName}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{TestName}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: Path.Combine(logDir, "testrun-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: template)
            .WriteTo.Sink(new ExtentSink())
            .WriteTo.Sink(new RunReportSink())
            .CreateLogger();
    }

    /// <summary>Returns a contextual logger that tags every entry with the test name.</summary>
    public static ILogger CreateLogger(string testName) => Log.ForContext("TestName", testName);
}
