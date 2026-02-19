#nullable enable
using Microsoft.Xna.Framework;
using Snails.Entities.Items;
using System.Collections.Generic;

namespace Snails.Entities.Ghost;

public struct RecordedFrame
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool IsSprinting;
}

public struct RecordedInteraction
{
    public int FrameIndex;
    public int StationIndex;
    public ItemType? HeldBefore;
    public ItemType? HeldAfter;
}

public class GhostRecording
{
    public List<RecordedFrame> Frames { get; } = new();
    public List<RecordedInteraction> Interactions { get; } = new();
}
