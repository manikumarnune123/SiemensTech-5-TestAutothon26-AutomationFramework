using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium.Appium;
using Serilog;
using Testhon.Framework.Configuration;
using Testhon.Framework.Driver;
using Testhon.Framework.Elements;
using Testhon.Framework.Logging;
using Testhon.Framework.Reporting;

namespace Testhon.Tests.Base;

/// <summary>
/// Reusable base class for Appium (mobile) tests. Handles the per-test Appium session lifecycle and
/// reuses the same logging and HTML reporting pipeline as the web <see cref="BaseTest"/>, capturing a
/// screenshot into the report on failure.
/// </summary>
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class MobileBaseTest
{
    private AppiumSession _session = null!;

    /// <summary>The live Appium session driver (Android or iOS).</summary>
    protected AppiumDriver Driver { get; private set; } = null!;

    /// <summary>Resilient, self-logging mobile element actions.</summary>
    protected IMobileElementActions Mobile { get; private set; } = null!;

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

        Log.Information("===== Starting mobile test: {TestName} =====", testName);

        _session = new AppiumSession(Log, Settings);
        await _session.InitializeAsync();

        Driver = _session.Driver;
        Mobile = new AppiumElementActions(Driver, Log, Settings);
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

        // Mark the body/teardown boundary before finalization logs so the report can pinpoint the
        // actual failing step rather than a later teardown line.
        if (record is not null)
        {
            record.BodyStepCount = record.StepCount;
        }

        try
        {
            var artifacts = await _session.FinalizeAsync(failed, testName);

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
            Log.Information("===== Finished mobile test: {TestName} =====", testName);
        }
    }
}
