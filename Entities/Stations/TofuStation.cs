#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class TofuStation : Station
{
    public override Color StationColor => new Color(230, 220, 180);
    public override string Label => "Tofu";

    public TofuStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem) => heldItem == null;

    public override void Interact(ref Item? playerItem)
    {
        TryGiveItem(ref playerItem, Item.Create(ItemType.Tofu)!);
    }
}
