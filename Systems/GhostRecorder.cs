#nullable enable
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Entities;
using Snails.Entities.Ghost;
using Snails.Entities.Items;

namespace Snails.Systems;

public enum RecordingState { Idle, Recording }

public class GhostRecorder
{
    public RecordingState State { get; private set; } = RecordingState.Idle;
    public float RecordingTimer { get; private set; }

    private GhostRecording? _current;
    private bool _prevSpaceDown;

    public GhostRecording? Update(KeyboardState keyState, Player player)
    {
        GhostRecording? completed = null;
        bool spaceDown = keyState.IsKeyDown(Keys.Space);
        bool spacePressed = spaceDown && !_prevSpaceDown;
        _prevSpaceDown = spaceDown;

        switch (State)
        {
            case RecordingState.Idle:
                if (spacePressed)
                {
                    _current = new GhostRecording();
                    RecordingTimer = 0f;
                    State = RecordingState.Recording;
                }
                break;

            case RecordingState.Recording:
                if (spacePressed || RecordingTimer >= GameConstants.MaxRecordingSeconds)
                {
                    completed = _current;
                    _current = null;
                    State = RecordingState.Idle;
                }
                break;
        }

        return completed;
    }

    public void CaptureFrame(Player player, float dt)
    {
        if (State != RecordingState.Recording || _current == null)
            return;

        _current.Frames.Add(new RecordedFrame
        {
            Position = player.Position,
            Velocity = player.Velocity,
            IsSprinting = player.IsSprinting
        });
        RecordingTimer += dt;
    }

    public void RecordInteraction(int stationIndex, ItemType? heldBefore, ItemType? heldAfter)
    {
        if (State != RecordingState.Recording || _current == null)
            return;

        _current.Interactions.Add(new RecordedInteraction
        {
            FrameIndex = _current.Frames.Count - 1,
            StationIndex = stationIndex,
            HeldBefore = heldBefore,
            HeldAfter = heldAfter
        });
    }
}
