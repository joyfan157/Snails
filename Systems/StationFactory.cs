#nullable enable
using System;
using Microsoft.Xna.Framework;
using Snails.Entities.Stations;

namespace Snails.Systems;

public static class StationFactory
{
    public static Station CreateStation(string type, Vector2 position,
        OrderManager? orderManager = null, ScoreManager? scoreManager = null)
    {
        return type switch
        {
            "RiceCooker" => new RiceCookerStation(position),
            "Salmon" => new SalmonStation(position),
            "Nori" => new NoriStation(position),
            "Tofu" => new TofuStation(position),
            "Dashi" => new DashiStation(position),
            "Chopping" => new ChoppingStation(position),
            "CuttingBoard" => new CuttingBoardStation(position),
            "Pot" => new PotStation(position),
            "Output" => new OutputStation(position,
                orderManager ?? throw new ArgumentNullException(nameof(orderManager)),
                scoreManager ?? throw new ArgumentNullException(nameof(scoreManager))),
            _ => throw new ArgumentException($"Unknown station type: {type}")
        };
    }
}
