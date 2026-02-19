#nullable enable
using Microsoft.Xna.Framework;
using Snails.Core;
using Snails.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Snails.Entities.Items;

public enum ItemType
{
    Rice,
    Salmon,
    ChoppedSalmon,
    Nigiri,
    Nori,
    Tofu,
    ChoppedTofu,
    Dashi,
    MakiRoll,
    MisoSoup
}

public class Item
{
    private static Dictionary<ItemType, ItemData>? _registry;

    public ItemType Type { get; }
    public Color DisplayColor { get; }
    public string DisplayName { get; }
    public int Size => GameConstants.ItemSize;

    public bool IsChoppable => RecipeManager.Instance.IsChoppable(Type);
    public bool IsServable => RecipeManager.Instance.IsServable(Type);

    private Item(ItemType type, Color color, string name)
    {
        Type = type;
        DisplayColor = color;
        DisplayName = name;
    }

    public static void LoadDefinitions(string jsonPath)
    {
        _registry = new Dictionary<ItemType, ItemData>();
        var json = File.ReadAllText(jsonPath);
        var entries = JsonDocument.Parse(json).RootElement;

        foreach (var entry in entries.EnumerateArray())
        {
            var type = Enum.Parse<ItemType>(entry.GetProperty("type").GetString()!);
            var name = entry.GetProperty("name").GetString()!;
            var colorArr = entry.GetProperty("color");
            var color = new Color(
                colorArr[0].GetInt32(),
                colorArr[1].GetInt32(),
                colorArr[2].GetInt32());
            _registry[type] = new ItemData(color, name);
        }
    }

    public static Item? Create(ItemType? type)
    {
        if (type == null) return null;
        if (_registry == null || !_registry.TryGetValue(type.Value, out var data))
            return null;
        return new Item(type.Value, data.Color, data.Name);
    }

    private record ItemData(Color Color, string Name);
}
