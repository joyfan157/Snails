using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Salmon : Item
{
    public override ItemType Type => ItemType.Salmon;
    public override Color DisplayColor => new Color(255, 150, 150);
    public override string DisplayName => "Salmon";
}
