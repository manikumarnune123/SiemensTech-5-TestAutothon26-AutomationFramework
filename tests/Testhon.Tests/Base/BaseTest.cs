using AventStack.ExtentReports;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Driver;
using Testhon.Framework.Elements;
using Testhon.Framework.Logging;
using Testhon.Framework.Reporting;

namespace Testhon.Tests.Base;

/// <summary>
/// Reusable base test class. Handles per-test browser session lifecycle, wires up logging and
/// reporting, and captures a screenshot into the report on failure.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BaseTest
{
    private PlaywrightDriver _driver = null!;

    protected IPage Page { get; private set; } = null!;
    protected IElementActions Elements { get; private set; } = null!;
    protected ILogger Log { get; private set; } = null!;
    protected RunSettings Settings => FrameworkConfig.Settings;

    [SetUp]
    public async Task SetUpAsync()
    {
        var currentTest = TestContext.CurrentContext.Test;
        var testName = currentTest.Name;

        Log = FrameworkLogger.CreateLogger(testName);

        var extentTest = ReportManager.CreateTest(testName);

        var record = RunReport.StartTest(testName);
        record.ClassName = currentTest.ClassName;
        record.Description = currentTest.Properties.Get("Description") as string;
        record.Categories = currentTest.Properties["Category"]?.Select(c => c?.ToString() ?? string.Empty).ToArray()
            ?? Array.Empty<string>();

        ReportContext.Register(currentTest.ID, extentTest, record);

        Log.Information("===== Starting test: {TestName} =====", testName);

        _driver = new PlaywrightDriver(Log, Settings);
        await _driver.InitializeAsync();

        Page = _driver.Page;
        Elements = new ElementActions(Page, Log, Settings);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        var currentTest = TestContext.CurrentContext.Test;
        var result = TestContext.CurrentContext.Result;
        var testName = currentTest.Name;
        var report = ReportContext.Current;
        var record = ReportContext.CurrentRecord;
        var failed = result.Outcome.Status == TestStatus.Failed;

        // Mark the body/teardown boundary before finalization logs (trace/video) so the report can
        // pinpoint the actual failing step rather than a later teardown line.
        if (record is not null)
        {
            record.BodyStepCount = record.StepCount;
        }

        try
        {
            var artifacts = await _driver.FinalizeAsync(failed, testName);

            if (record is not null)
            {
                record.ScreenshotBase64 = artifacts.ScreenshotBase64;
                record.TracePath = artifacts.TracePath;
                record.VideoPath = artifacts.VideoPath;
            }

            switch (result.Outcome.Status)
            {
                case TestStatus.Failed:
                    Log.Error("Test FAILED: {TestName} | {Message}", testName, result.Message);
                    if (record is not null)
                    {
                        record.Outcome = TestOutcome.Failed;
                        record.ErrorMessage = result.Message;
                        record.StackTrace = result.StackTrace;
                    }
                    if (artifacts.ScreenshotBase64 is not null)
                    {
                        report?.Fail(
                            result.Message ?? "Test failed",
                            MediaEntityBuilder.CreateScreenCaptureFromBase64String(artifacts.ScreenshotBase64, testName).Build());
                    }
                    else
                    {
                        report?.Fail(result.Message ?? "Test failed");
                    }
                    break;

                case TestStatus.Skipped:
                    Log.Warning("Test SKIPPED: {TestName}", testName);
                    if (record is not null)
                    {
                        record.Outcome = TestOutcome.Skipped;
                        record.ErrorMessage = result.Message;
                    }
                    report?.Skip(result.Message ?? "Test skipped");
                    break;

                default:
                    Log.Information("Test PASSED: {TestName}", testName);
                    if (record is not null)
                    {
                        record.Outcome = TestOutcome.Passed;
                    }
                    report?.Pass("Test passed");
                    break;
            }

            if (artifacts.TracePath is not null)
            {
                report?.Info($"Trace: {artifacts.TracePath}");
            }

            if (artifacts.VideoPath is not null)
            {
                report?.Info($"Video: {artifacts.VideoPath}");
            }
        }
        finally
        {
            if (record is not null)
            {
                RunReport.Complete(record);
            }
            ReportContext.Unregister(currentTest.ID);
            Log.Information("===== Finished test: {TestName} =====", testName);
        }
    }
}
