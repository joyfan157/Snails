#nullable enable
using System;
using System.IO;
using System.Text.Json;
using Snails.Data;

namespace Snails.Systems;

public static class LevelLoader
{
    public static LevelData LoadLevel(int levelNumber)
    {
        string filePath = Path.Combine("Data", "Levels", $"level{levelNumber}.json");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Level file not found: {filePath}");

        string jsonText = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        LevelData? level = JsonSerializer.Deserialize<LevelData>(jsonText, options);

        if (level == null)
            throw new InvalidDataException($"Failed to parse level: {filePath}");

        // Validation
        if (level.Stations.Count == 0)
            throw new InvalidDataException($"Level {levelNumber} has no stations!");

        return level;
    }
}
