using Microsoft.Xna.Framework;
using Snails.Core;

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
}
