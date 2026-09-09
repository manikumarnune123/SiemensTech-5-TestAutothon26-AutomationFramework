using NUnit.Framework;
using Testhon.Framework.Configuration;
using Testhon.Framework.Logging;
using Testhon.Framework.Reporting;

// No namespace on purpose: a SetUpFixture in the global namespace runs once for the whole assembly.

/// <summary>Assembly-wide bootstrap: configures logging and the HTML report, then flushes it.</summary>
[SetUpFixture]
public sealed class GlobalSetup
{
    [OneTimeSetUp]
    public void BeforeAllTests()
    {
        FrameworkLogger.Initialize(FrameworkConfig.Settings);
        ReportManager.Init();

        // NUnit's TestContext flows across setup/body/teardown and async awaits, unlike an
        // AsyncLocal set in [SetUp]; use it to attribute every log step to the running test.
        ReportContext.CurrentKeyResolver = () => TestContext.CurrentContext.Test.ID;
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        ReportManager.Flush();
        Serilog.Log.CloseAndFlush();
    }
}
