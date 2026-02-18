#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class SalmonStation : Station
{
    public override Color StationColor => new Color(255, 120, 120);
    public override string Label => "Salmon";

    public SalmonStation(Vector2 position) : base(position) { }

    public override void Interact(ref Item? playerItem)
    {
        if (playerItem == null)
            playerItem = new Salmon();
    }
}
