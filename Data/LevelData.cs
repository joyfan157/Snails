using System.Collections.Generic;

namespace Snails.Data;

public class LevelData
{
    public int LevelNumber { get; set; }
    public string Name { get; set; } = "";
    public int ScoreGoal { get; set; }
    public float TimeLimit { get; set; }
    public List<StationData> Stations { get; set; } = new();
    public List<string> AllowedDishes { get; set; } = new();
    public float OrderSpawnMin { get; set; }
    public float OrderSpawnMax { get; set; }
    public float PlayerStartX { get; set; }
    public float PlayerStartY { get; set; }
}
