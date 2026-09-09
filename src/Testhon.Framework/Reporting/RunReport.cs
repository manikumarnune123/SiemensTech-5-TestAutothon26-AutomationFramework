using System.Collections.Concurrent;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Thread-safe, in-memory collector of everything that happens during a run. Populated by the
/// test lifecycle and rendered into a self-contained HTML report by <see cref="HtmlReportGenerator"/>.
/// </summary>
public static class RunReport
{
    private static readonly ConcurrentQueue<TestRecord> _tests = new();
    private static readonly ConcurrentDictionary<string, string> _systemInfo = new();

    public static DateTimeOffset StartTime { get; private set; } = DateTimeOffset.Now;
    public static DateTimeOffset EndTime { get; private set; } = DateTimeOffset.Now;

    public static void Start() => StartTime = DateTimeOffset.Now;

    public static void AddSystemInfo(string key, string value) => _systemInfo[key] = value;

    /// <summary>Creates a record for a starting test. Add it back with <see cref="Complete"/> when done.</summary>
    public static TestRecord StartTest(string name) => new()
    {
        Name = name,
        StartTime = DateTimeOffset.Now
    };

    public static void Complete(TestRecord record)
    {
        record.EndTime = DateTimeOffset.Now;
        _tests.Enqueue(record);
    }

    /// <summary>Snapshot of all completed tests, ordered by start time.</summary>
    public static IReadOnlyList<TestRecord> Tests =>
        _tests.OrderBy(t => t.StartTime).ToArray();

    public static IReadOnlyDictionary<string, string> SystemInfo => _systemInfo;

    public static void Stop() => EndTime = DateTimeOffset.Now;
}
