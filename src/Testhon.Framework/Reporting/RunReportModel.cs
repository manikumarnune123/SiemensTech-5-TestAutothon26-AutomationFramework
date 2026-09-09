namespace Testhon.Framework.Reporting;

/// <summary>Severity of a single step captured during a test.</summary>
public enum StepStatus
{
    Info,
    Pass,
    Warning,
    Fail
}

/// <summary>Final outcome of a test.</summary>
public enum TestOutcome
{
    Passed,
    Failed,
    Skipped
}

/// <summary>One line in a test's step timeline.</summary>
public sealed class StepRecord
{
    public DateTimeOffset Timestamp { get; init; }
    public StepStatus Status { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>All data captured for a single test, consumed by the HTML report generator.</summary>
public sealed class TestRecord
{
    private readonly object _stepsLock = new();
    private readonly List<StepRecord> _steps = new();

    public string Name { get; init; } = string.Empty;
    public string? ClassName { get; set; }
    public string? Description { get; set; }
    public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();

    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public TimeSpan Duration => EndTime > StartTime ? EndTime - StartTime : TimeSpan.Zero;

    public TestOutcome Outcome { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }

    public string? ScreenshotBase64 { get; set; }
    public string? TracePath { get; set; }
    public string? VideoPath { get; set; }

    /// <summary>Number of steps logged up to the end of the test body (before teardown finalization).
    /// Used to pinpoint the failing step and separate teardown/context lines. -1 = not marked.</summary>
    public int BodyStepCount { get; set; } = -1;

    public int StepCount
    {
        get
        {
            lock (_stepsLock)
            {
                return _steps.Count;
            }
        }
    }

    public IReadOnlyList<StepRecord> Steps
    {
        get
        {
            lock (_stepsLock)
            {
                return _steps.ToArray();
            }
        }
    }

    public void AddStep(StepStatus status, string message)
    {
        lock (_stepsLock)
        {
            _steps.Add(new StepRecord
            {
                Timestamp = DateTimeOffset.Now,
                Status = status,
                Message = message
            });
        }
    }
}
