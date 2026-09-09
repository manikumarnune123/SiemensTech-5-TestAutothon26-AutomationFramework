using System.Collections.Concurrent;
using AventStack.ExtentReports;

namespace Testhon.Framework.Reporting;

/// <summary>
/// Resolves the reporting objects (Extent test node and custom <see cref="TestRecord"/>) for the
/// test currently executing.
/// <para>
/// A test runner such as NUnit runs <c>[SetUp]</c>, the test body and <c>[TearDown]</c> as separate
/// awaited operations, so an <see cref="AsyncLocal{T}"/> value set in setup does NOT flow into the
/// body or teardown (copy-on-write semantics). To capture every step reliably we key the reporting
/// objects by the runner's stable per-test id and resolve them live through
/// <see cref="CurrentKeyResolver"/>, which the test host injects (e.g. NUnit's
/// <c>TestContext.CurrentContext.Test.ID</c>). This keeps the framework free of any test-runner
/// dependency.
/// </para>
/// </summary>
public static class ReportContext
{
    private static readonly ConcurrentDictionary<string, ExtentTest> _extentByKey = new();
    private static readonly ConcurrentDictionary<string, TestRecord> _recordByKey = new();

    /// <summary>Injected by the test host to identify the running test (e.g. the NUnit test id).</summary>
    public static Func<string?>? CurrentKeyResolver { get; set; }

    private static string? CurrentKey => CurrentKeyResolver?.Invoke();

    public static ExtentTest? Current =>
        CurrentKey is { } key && _extentByKey.TryGetValue(key, out var test) ? test : null;

    public static TestRecord? CurrentRecord =>
        CurrentKey is { } key && _recordByKey.TryGetValue(key, out var record) ? record : null;

    /// <summary>Associates reporting objects with a test id for the duration of that test.</summary>
    public static void Register(string key, ExtentTest test, TestRecord record)
    {
        _extentByKey[key] = test;
        _recordByKey[key] = record;
    }

    public static void Unregister(string key)
    {
        _extentByKey.TryRemove(key, out _);
        _recordByKey.TryRemove(key, out _);
    }
}
