using NUnit.Framework;

// Run test fixtures in parallel; tests inside a fixture run sequentially so instance
// fields (Page, Driver, ...) in BaseTest remain isolated per fixture.
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]
