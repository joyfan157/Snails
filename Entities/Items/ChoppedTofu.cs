using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class ChoppedTofu : Item
{
    public override ItemType Type => ItemType.ChoppedTofu;
    public override Color DisplayColor => new Color(220, 210, 170);
    public override string DisplayName => "Chopped Tofu";
}
