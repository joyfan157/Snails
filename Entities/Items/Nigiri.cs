using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Nigiri : Item
{
    public override ItemType Type => ItemType.Nigiri;
    public override Color DisplayColor => new Color(255, 200, 150);
    public override string DisplayName => "Nigiri";
}
