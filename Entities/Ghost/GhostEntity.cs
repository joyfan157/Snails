#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Entities.Stations;
using System.Collections.Generic;

namespace Snails.Entities.Ghost;

public class GhostEntity
{
    private readonly GhostRecording _recording;
    private readonly Color _color;
    private readonly List<Station> _stations;

    private int _currentFrame;
    private int _nextInteractionIdx;
    private Item? _heldItem;
    private bool _isWaiting;
    private bool _isReturning;
    private float _waitPulse;

    public Vector2 Position { get; private set; }

    public GhostEntity(GhostRecording recording, Color color, List<Station> stations)
    {
        _recording = recording;
        _color = color;
        _stations = stations;

        if (_recording.Frames.Count > 0)
            Position = _recording.Frames[0].Position;
    }

    public void Update(GameTime gameTime)
    {
        if (_recording.Frames.Count == 0)
            return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _waitPulse += dt * 3f;

        // Returning to start position after loop ends
        if (_isReturning)
        {
            var start = _recording.Frames[0].Position;
            var diff = start - Position;
            float dist = diff.Length();

            if (dist <= GameConstants.GhostReturnArrivalDist)
            {
                // Arrived — begin new loop
                Position = start;
                _isReturning = false;
                _currentFrame = 0;
                _nextInteractionIdx = 0;
            }
            else
            {
                // Move toward start
                var dir = diff / dist;
                Position += dir * GameConstants.GhostReturnSpeed * dt;
            }
            return;
        }

        // Check if the current frame has a pending interaction
        if (_nextInteractionIdx < _recording.Interactions.Count)
        {
            var interaction = _recording.Interactions[_nextInteractionIdx];
            if (interaction.FrameIndex == _currentFrame)
            {
                if (TryExecuteInteraction(interaction))
                {
                    _nextInteractionIdx++;
                    _isWaiting = false;
                }
                else
                {
                    _isWaiting = true;
                    return; // Don't advance frame
                }
            }
        }

        // Advance to next frame
        _currentFrame++;
        if (_currentFrame >= _recording.Frames.Count)
        {
            // Loop ended — drop held item and return to start
            _heldItem = null;
            _isWaiting = false;
            _isReturning = true;
            return;
        }

        var frame = _recording.Frames[_currentFrame];
        Position = frame.Position;
        _isWaiting = false;
    }

    private bool TryExecuteInteraction(RecordedInteraction interaction)
    {
        // Verify ghost holds expected item type
        ItemType? currentHeld = _heldItem?.Type;
        if (currentHeld != interaction.HeldBefore)
            return false;

        if (interaction.StationIndex < 0 || interaction.StationIndex >= _stations.Count)
            return false;

        var station = _stations[interaction.StationIndex];

        // Pre-check if station can accept this interaction
        if (!station.CanInteract(_heldItem))
            return false;

        // Execute the interaction
        var item = _heldItem;
        station.Interact(ref item);
        _heldItem = item;

        return true;
    }

    public void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        if (_recording.Frames.Count == 0)
            return;

        int size = GameConstants.PlayerSize;
        var rect = new Rectangle(
            (int)(Position.X - size / 2),
            (int)(Position.Y - size / 2),
            size, size);

        var ghostColor = new Color((int)_color.R, (int)_color.G, (int)_color.B, GameConstants.GhostAlpha);
        textures.DrawRect(spriteBatch, rect, ghostColor);

        // Draw held item (semi-transparent)
        if (_heldItem != null)
        {
            var itemRect = new Rectangle(
                (int)(Position.X - _heldItem.Size / 2),
                (int)(Position.Y - size / 2 - _heldItem.Size - 4),
                _heldItem.Size, _heldItem.Size);
            var itemColor = new Color(
                (int)_heldItem.DisplayColor.R,
                (int)_heldItem.DisplayColor.G,
                (int)_heldItem.DisplayColor.B,
                GameConstants.GhostAlpha);
            textures.DrawRect(spriteBatch, itemRect, itemColor);
        }

        // Show waiting indicator
        if (_isWaiting)
        {
            float pulse = (float)System.Math.Sin(_waitPulse) * 0.5f + 0.5f;
            var dotColor = new Color(255, 255, 255, (int)(pulse * 200));
            var dotRect = new Rectangle(
                (int)(Position.X - 8),
                (int)(Position.Y - size / 2 - 16),
                16, 8);
            textures.DrawRect(spriteBatch, dotRect, dotColor);
        }
    }
}
