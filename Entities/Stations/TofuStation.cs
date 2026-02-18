#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class TofuStation : Station
{
    public override Color StationColor => new Color(230, 220, 180);
    public override string Label => "Tofu";

    public TofuStation(Vector2 position) : base(position) { }

    public override void Interact(ref Item? playerItem)
    {
        if (playerItem == null)
            playerItem = new Tofu();
    }
}
