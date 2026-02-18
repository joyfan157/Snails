using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class Nori : Item
{
    public override ItemType Type => ItemType.Nori;
    public override Color DisplayColor => new Color(50, 80, 50);
    public override string DisplayName => "Nori";
}
