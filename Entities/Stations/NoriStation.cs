#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class NoriStation : Station
{
    public override Color StationColor => new Color(70, 110, 70);
    public override string Label => "Nori";

    public NoriStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem) => heldItem == null;

    public override void Interact(ref Item? playerItem)
    {
        TryGiveItem(ref playerItem, new Nori());
    }
}
