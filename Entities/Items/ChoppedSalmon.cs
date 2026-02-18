using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class ChoppedSalmon : Item
{
    public override ItemType Type => ItemType.ChoppedSalmon;
    public override Color DisplayColor => new Color(200, 80, 80);
    public override string DisplayName => "Chopped Salmon";
}
