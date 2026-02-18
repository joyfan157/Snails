namespace Snails.Core;

public static class GameConstants
{
    public const int WindowWidth = 1024;
    public const int WindowHeight = 768;

    public const int StationSize = 64;
    public const int ItemSize = 20;
    public const int PlayerSize = 32;

    public const float PlayerSpeed = 200f;
    public const float InteractRange = 60f;

    public const int HudHeight = 100;

    public const float RiceCookTime = 3f;
    public const float ChopTime = 2f;
    public const float PotCookTime = 3f;

    public const float OrderSpawnIntervalMax = 15f;
    public const float OrderSpawnIntervalMin = 8f;
    public const float OrderTimeLimit = 60f;
    public const int MaxActiveOrders = 5;

    public const int ScoreFulfilled = 100;
    public const int ScoreNoOrder = 25;
}
