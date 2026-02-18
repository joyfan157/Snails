using Microsoft.Xna.Framework;

namespace Snails.Entities.Items;

public class MisoSoup : Item
{
    public override ItemType Type => ItemType.MisoSoup;
    public override Color DisplayColor => new Color(200, 170, 80);
    public override string DisplayName => "Miso Soup";
}
