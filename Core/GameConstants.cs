using Microsoft.Xna.Framework;

namespace Snails.Core;

public static class GameConstants
{
    public const int WindowWidth = 1280;
    public const int WindowHeight = 900;

    public const int StationSize = 64;
    public const int ItemSize = 20;
    public const int PlayerSize = 32;

    public const float PlayerSpeed = 200f;
    public const float SprintSpeed = 450f;
    public const float SprintBurstSpeed = 300f;  // Instant speed boost when sprint starts
    public const float SprintAcceleration = 1400f;
    public const float WalkAcceleration = 5000f;
    public const float SprintFriction = 0.3f;   // Very low friction while sprinting (ice-like)
    public const float WalkFriction = 25f;       // Very high friction at walk speed (stiff/instant stop)
    public const float SprintDamping = 1.5f;     // Slow damping when transitioning from sprint to walk
    public const float BounceRestitution = 0.8f;  // How much velocity preserved on bounce
    public const float MaxStamina = 100f;
    public const float StaminaDrainRate = 30f;    // Per second while sprinting
    public const float StaminaRechargeRate = 20f; // Per second while not sprinting
    public const int ObstacleSize = 48;
    public const float InteractRange = 60f;

    public const int HudHeight = 100;

    public const float RiceCookTime = 3f;
    public const float ChopTime = 2f;
    public const float PotCookTime = 3f;

    public const float OrderSpawnIntervalMax = 15f;
    public const float OrderSpawnIntervalMin = 10f;
    public const float OrderTimeLimit = 60f;
    public const int MaxActiveOrders = 5;

    public const int ScoreFulfilled = 100;
    public const int ScoreNoOrder = 25;

    // Ghost recording
    public const float MaxRecordingSeconds = 30f;
    public const int GhostAlpha = 120;
    public const float GhostReturnSpeed = 150f;
    public const float GhostReturnArrivalDist = 4f;

    public static readonly Color[] GhostColors = new[]
    {
        Color.Red, Color.Green, Color.Blue, Color.Yellow,
        Color.Magenta, Color.Cyan, Color.Orange, Color.Purple
    };
}
