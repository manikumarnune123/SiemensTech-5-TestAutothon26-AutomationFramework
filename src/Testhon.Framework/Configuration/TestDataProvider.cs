using System.Text.Json;

namespace Testhon.Framework.Configuration;

/// <summary>
/// Loads JSON test data (deployed next to the test assembly) into strongly-typed models.
/// Keeps input data out of test code and easy to maintain per environment.
/// </summary>
public static class TestDataProvider
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Load<T>(string relativePath)
    {
        var path = Path.Combine(AppContext.BaseDirectory, relativePath);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Test data file not found: {path}");
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, Options)
            ?? throw new InvalidOperationException($"Failed to deserialize test data from '{path}'.");
    }
}
