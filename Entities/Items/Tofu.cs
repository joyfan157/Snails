using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Tofu : Item
{
    public override ItemType Type => ItemType.Tofu;
    public override Color DisplayColor => new Color(245, 235, 200);
    public override string DisplayName => "Tofu";
}
