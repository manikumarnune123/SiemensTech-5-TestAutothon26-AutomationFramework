using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Runtime.InteropServices;
using Testhon.Framework.Configuration;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Thread-safe wrapper around a single <see cref="ExtentReports"/> instance that produces an
/// interactive HTML (Spark) report. Initialized once per run and flushed at the end. Also drives
/// the bespoke self-contained HTML report via <see cref="RunReport"/> / <see cref="HtmlReportGenerator"/>.
/// </summary>
public static class ReportManager
{
    private static readonly object _lock = new();
    private static ExtentReports? _extent;

    public static void Init()
    {
        lock (_lock)
        {
            if (_extent is not null)
            {
                return;
            }

            var reportDir = FrameworkConfig.ResultsDirectory;
            Directory.CreateDirectory(reportDir);

            var spark = new ExtentSparkReporter(Path.Combine(reportDir, "index.html"));

            _extent = new ExtentReports();
            _extent.AttachReporter(spark);
            _extent.AddSystemInfo("Environment", FrameworkConfig.Environment);
            _extent.AddSystemInfo("Platform", FrameworkConfig.Settings.Platform.ToString());
            _extent.AddSystemInfo("Browser", FrameworkConfig.Settings.Browser.ToString());
            _extent.AddSystemInfo("Base URL", FrameworkConfig.Settings.BaseUrl);

            RunReport.Start();
            RunReport.AddSystemInfo("Environment", FrameworkConfig.Environment);
            RunReport.AddSystemInfo("Platform", FrameworkConfig.Settings.Platform.ToString());
            RunReport.AddSystemInfo("Browser", FrameworkConfig.Settings.Browser.ToString());
            RunReport.AddSystemInfo("Base URL", FrameworkConfig.Settings.BaseUrl);
            RunReport.AddSystemInfo("Headless", FrameworkConfig.Settings.Headless ? "Yes" : "No");
            RunReport.AddSystemInfo("Machine", Environment.MachineName);
            RunReport.AddSystemInfo("OS", RuntimeInformation.OSDescription.Trim());
            RunReport.AddSystemInfo("User", Environment.UserName);
        }
    }

    public static ExtentTest CreateTest(string name)
    {
        lock (_lock)
        {
            var extent = _extent ?? throw new InvalidOperationException(
                $"{nameof(ReportManager)} is not initialized. Call {nameof(Init)}() in a global setup.");
            return extent.CreateTest(name);
        }
    }

    public static void Flush()
    {
        lock (_lock)
        {
            _extent?.Flush();

            var reportDir = FrameworkConfig.ResultsDirectory;
            HtmlReportGenerator.Generate(Path.Combine(reportDir, "testhon-report.html"));
        }
    }
}
