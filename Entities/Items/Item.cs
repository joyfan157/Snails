#nullable enable
using Microsoft.Xna.Framework;
using Snails.Core;
using Snails.Systems;

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

public abstract class Item
{
    public abstract ItemType Type { get; }
    public abstract Color DisplayColor { get; }
    public abstract string DisplayName { get; }
    public int Size => GameConstants.ItemSize;

    public bool IsChoppable => RecipeManager.Instance.IsChoppable(Type);
    public bool IsServable => RecipeManager.Instance.IsServable(Type);

    public Item? CreateChoppedResult()
    {
        var recipe = RecipeManager.Instance.GetChoppingRecipe(Type);
        return recipe != null ? Create(recipe.Output) : null;
    }

    public static Item? Create(ItemType? type) => type switch
    {
        ItemType.Rice => new Rice(),
        ItemType.Salmon => new Salmon(),
        ItemType.ChoppedSalmon => new ChoppedSalmon(),
        ItemType.Nigiri => new Nigiri(),
        ItemType.Nori => new Nori(),
        ItemType.Tofu => new Tofu(),
        ItemType.ChoppedTofu => new ChoppedTofu(),
        ItemType.Dashi => new Dashi(),
        ItemType.MakiRoll => new MakiRoll(),
        ItemType.MisoSoup => new MisoSoup(),
        null => null,
        _ => null
    };
}
