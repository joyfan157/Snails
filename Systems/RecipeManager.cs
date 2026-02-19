#nullable enable
using Snails.Entities.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Snails.Systems;

public record ChoppingRecipe(ItemType Input, ItemType Output, float CookTime);
public record CombiningRecipe(HashSet<ItemType> Inputs, ItemType Output, string Station, float? CookTime);

public class RecipeManager
{
    private static RecipeManager? _instance;
    public static RecipeManager Instance => _instance ?? throw new InvalidOperationException("RecipeManager not initialized");

    public List<ChoppingRecipe> ChoppingRecipes { get; } = new();
    public List<CombiningRecipe> CombiningRecipes { get; } = new();
    public HashSet<ItemType> ServableItems { get; } = new();

    public static void Initialize(string jsonPath)
    {
        _instance = new RecipeManager();
        var json = File.ReadAllText(jsonPath);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        foreach (var entry in root.GetProperty("choppingRecipes").EnumerateArray())
        {
            var input = Enum.Parse<ItemType>(entry.GetProperty("input").GetString()!);
            var output = Enum.Parse<ItemType>(entry.GetProperty("output").GetString()!);
            var cookTime = entry.GetProperty("cookTime").GetSingle();
            _instance.ChoppingRecipes.Add(new ChoppingRecipe(input, output, cookTime));
        }

        foreach (var entry in root.GetProperty("combiningRecipes").EnumerateArray())
        {
            var inputs = new HashSet<ItemType>();
            foreach (var inp in entry.GetProperty("inputs").EnumerateArray())
                inputs.Add(Enum.Parse<ItemType>(inp.GetString()!));
            var output = Enum.Parse<ItemType>(entry.GetProperty("output").GetString()!);
            var station = entry.GetProperty("station").GetString()!;
            float? cookTime = entry.TryGetProperty("cookTime", out var ct) ? ct.GetSingle() : null;
            _instance.CombiningRecipes.Add(new CombiningRecipe(inputs, output, station, cookTime));
        }

        foreach (var entry in root.GetProperty("servableItems").EnumerateArray())
            _instance.ServableItems.Add(Enum.Parse<ItemType>(entry.GetString()!));
    }

    public ChoppingRecipe? GetChoppingRecipe(ItemType input)
        => ChoppingRecipes.FirstOrDefault(r => r.Input == input);

    public bool IsChoppable(ItemType type)
        => ChoppingRecipes.Any(r => r.Input == type);

    public CombiningRecipe? FindCombiningRecipe(ItemType first, ItemType second, string station)
    {
        var pair = new HashSet<ItemType> { first, second };
        return CombiningRecipes.FirstOrDefault(r => r.Station == station && r.Inputs.SetEquals(pair));
    }

    public bool IsServable(ItemType type)
        => ServableItems.Contains(type);
}
