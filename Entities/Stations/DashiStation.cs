#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class DashiStation : Station
{
    public override Color StationColor => new Color(160, 140, 100);
    public override string Label => "Dashi";

    public DashiStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem) => heldItem == null;

    public override void Interact(ref Item? playerItem)
    {
        TryGiveItem(ref playerItem, new Dashi());
    }
}
