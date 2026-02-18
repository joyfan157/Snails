using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class MakiRoll : Item
{
    public override ItemType Type => ItemType.MakiRoll;
    public override Color DisplayColor => new Color(100, 180, 100);
    public override string DisplayName => "Maki Roll";
}
