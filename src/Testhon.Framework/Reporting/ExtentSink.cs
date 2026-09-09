using AventStack.ExtentReports;
using Serilog.Core;
using Serilog.Events;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Serilog sink that mirrors log events (Information and above) as steps on the current
/// <see cref="ExtentTest"/>. This is what makes "every method step" appear in the HTML report
/// without the page/element code having any direct dependency on the reporter.
/// </summary>
public sealed class ExtentSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var test = ReportContext.Current;
        if (test is null || logEvent.Level < LogEventLevel.Information)
        {
            return;
        }

        var status = logEvent.Level switch
        {
            LogEventLevel.Warning => Status.Warning,
            LogEventLevel.Error or LogEventLevel.Fatal => Status.Error,
            _ => Status.Info
        };

        test.Log(status, logEvent.RenderMessage());
    }
}
