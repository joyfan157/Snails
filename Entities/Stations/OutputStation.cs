#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;
using Snails.Systems;

namespace Snails.Entities.Stations;

public class OutputStation : Station
{
    private readonly OrderManager _orderManager;
    private readonly ScoreManager _scoreManager;

    public override Color StationColor => new Color(100, 200, 100);
    public override string Label => "Serve";

    public OutputStation(Vector2 position, OrderManager orderManager, ScoreManager scoreManager)
        : base(position)
    {
        _orderManager = orderManager;
        _scoreManager = scoreManager;
    }

    public override bool CanInteract(Item? heldItem)
        => heldItem != null && heldItem.IsServable;

    public override void Interact(ref Item? playerItem)
    {
        if (playerItem == null || !playerItem.IsServable)
            return;

        if (_orderManager.TryFulfillOrder(playerItem.Type))
            _scoreManager.AddScore(Core.GameConstants.ScoreFulfilled);
        else
            _scoreManager.AddScore(Core.GameConstants.ScoreNoOrder);

        playerItem = null;
    }
}
