using Serilog.Core;
using Serilog.Events;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Serilog sink that mirrors log events (Information and above) as steps on the current
/// <see cref="TestRecord"/>, feeding the custom HTML report's step timeline. Runs alongside
/// <see cref="ExtentSink"/> with no coupling to the page/element code.
/// </summary>
public sealed class RunReportSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var record = ReportContext.CurrentRecord;
        if (record is null || logEvent.Level < LogEventLevel.Information)
        {
            return;
        }

        var status = logEvent.Level switch
        {
            LogEventLevel.Warning => StepStatus.Warning,
            LogEventLevel.Error or LogEventLevel.Fatal => StepStatus.Fail,
            _ => StepStatus.Info
        };

        record.AddStep(status, logEvent.RenderMessage());
    }
}
