using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Rice : Item
{
    public override ItemType Type => ItemType.Rice;
    public override Color DisplayColor => Color.White;
    public override string DisplayName => "Rice";
}
