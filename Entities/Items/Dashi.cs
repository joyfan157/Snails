using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Dashi : Item
{
    public override ItemType Type => ItemType.Dashi;
    public override Color DisplayColor => new Color(180, 160, 120);
    public override string DisplayName => "Dashi";
}
