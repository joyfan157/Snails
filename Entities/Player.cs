#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Entities.Stations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snails.Entities;

public class Player
{
    public Vector2 Position;
    public Item? HeldItem;
    public Vector2 Velocity;
    public float Stamina;
    public bool IsSprinting;
    public bool StaminaDepleted; // Can't sprint until stamina is full again
    private bool _wasSprinting;

    public event Action<int, ItemType?, ItemType?>? OnStationInteraction;

    private MouseState _prevMouseState;
    private KeyboardState _prevKeyState;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
        Stamina = GameConstants.MaxStamina;
    }

    public void Update(GameTime gameTime, List<Station> stations, List<Obstacle> obstacles)
    {
        var keyState = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Sprint input
        bool wantsSprint = keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift);
        IsSprinting = wantsSprint && !StaminaDepleted && Stamina > 0;

        // Stamina management
        if (IsSprinting)
        {
            Stamina -= GameConstants.StaminaDrainRate * dt;
            if (Stamina <= 0)
            {
                Stamina = 0;
                StaminaDepleted = true;
                IsSprinting = false;
            }
        }
        else
        {
            Stamina += GameConstants.StaminaRechargeRate * dt;
            if (Stamina >= GameConstants.MaxStamina)
            {
                Stamina = GameConstants.MaxStamina;
                StaminaDepleted = false;
            }
        }

        // Movement input direction
        var input = Vector2.Zero;
        if (keyState.IsKeyDown(Keys.W)) input.Y -= 1;
        if (keyState.IsKeyDown(Keys.S)) input.Y += 1;
        if (keyState.IsKeyDown(Keys.A)) input.X -= 1;
        if (keyState.IsKeyDown(Keys.D)) input.X += 1;
        if (input != Vector2.Zero)
            input.Normalize();

        float speed = Velocity.Length();

        // Instant burst when sprint starts
        if (IsSprinting && !_wasSprinting)
        {
            Vector2 burstDir = input != Vector2.Zero ? input
                : (Velocity != Vector2.Zero ? Vector2.Normalize(Velocity) : Vector2.Zero);
            if (burstDir != Vector2.Zero)
                Velocity = burstDir * GameConstants.SprintBurstSpeed;
        }
        _wasSprinting = IsSprinting;

        if (IsSprinting)
        {
            // Sprinting: slippery, hard to control — input adds force but momentum dominates
            Velocity += input * GameConstants.SprintAcceleration * dt;

            // Very light friction — player slides like on ice
            if (Velocity != Vector2.Zero)
            {
                float frictionFactor = 1f - GameConstants.SprintFriction * dt;
                Velocity *= Math.Max(frictionFactor, 0f);
            }

            // Cap at sprint speed
            if (Velocity.Length() > GameConstants.SprintSpeed)
            {
                Velocity.Normalize();
                Velocity *= GameConstants.SprintSpeed;
            }
        }
        else
        {
            // Not sprinting: stiff controls, speed capped to walk speed
            if (input != Vector2.Zero)
            {
                // Snap velocity toward input direction for stiff feel
                Vector2 targetVelocity = input * GameConstants.PlayerSpeed;
                Velocity = Vector2.Lerp(Velocity, targetVelocity, Math.Min(GameConstants.WalkAcceleration * dt / GameConstants.PlayerSpeed, 1f));
            }
            else
            {
                // No input: stop almost instantly
                float frictionFactor = 1f - GameConstants.WalkFriction * dt;
                Velocity *= Math.Max(frictionFactor, 0f);

                if (Velocity.Length() < 10f)
                    Velocity = Vector2.Zero;
            }
        }

        // Apply velocity
        Position += Velocity * dt;

        // Whether we're moving fast enough to bounce (sprinting or coasting from sprint)
        bool canBounce = IsSprinting || Velocity.Length() > GameConstants.PlayerSpeed + 10f;

        // Collect all solid rectangles
        var solids = new List<Rectangle>(stations.Count + obstacles.Count);
        foreach (var station in stations)
            solids.Add(station.Bounds);
        foreach (var obstacle in obstacles)
            solids.Add(obstacle.Bounds);

        // Resolve collisions (multiple passes for corner cases)
        for (int pass = 0; pass < 3; pass++)
        {
            bool anyCollision = false;
            foreach (var solid in solids)
            {
                if (GetBounds().Intersects(solid))
                {
                    ResolveCollision(solid, canBounce);
                    anyCollision = true;
                }
            }
            if (!anyCollision) break;
        }

        // Wall boundaries
        float half = GameConstants.PlayerSize / 2f;
        if (Position.X - half < 0)
        {
            Position.X = half;
            Velocity.X = canBounce ? Math.Abs(Velocity.X) * GameConstants.BounceRestitution : 0;
        }
        else if (Position.X + half > GameConstants.WindowWidth)
        {
            Position.X = GameConstants.WindowWidth - half;
            Velocity.X = canBounce ? -Math.Abs(Velocity.X) * GameConstants.BounceRestitution : 0;
        }

        if (Position.Y - half < GameConstants.HudHeight)
        {
            Position.Y = GameConstants.HudHeight + half;
            Velocity.Y = canBounce ? Math.Abs(Velocity.Y) * GameConstants.BounceRestitution : 0;
        }
        else if (Position.Y + half > GameConstants.WindowHeight)
        {
            Position.Y = GameConstants.WindowHeight - half;
            Velocity.Y = canBounce ? -Math.Abs(Velocity.Y) * GameConstants.BounceRestitution : 0;
        }

        // Click interaction: must click a station within interact range
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed &&
            _prevMouseState.LeftButton == ButtonState.Released)
        {
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            var clicked = stations
                .Where(s => s.Bounds.Contains(mousePoint) && s.DistanceTo(Position) <= GameConstants.InteractRange)
                .FirstOrDefault();

            if (clicked != null && clicked.CanInteract(HeldItem))
            {
                var heldBefore = HeldItem?.Type;
                var item = HeldItem;
                clicked.Interact(ref item);
                HeldItem = item;
                var heldAfter = HeldItem?.Type;
                int stationIndex = stations.IndexOf(clicked);
                OnStationInteraction?.Invoke(stationIndex, heldBefore, heldAfter);
            }
        }

        // Trash held item with Q
        if (keyState.IsKeyDown(Keys.Q) && !_prevKeyState.IsKeyDown(Keys.Q) && HeldItem != null)
            HeldItem = null;

        _prevMouseState = mouseState;
        _prevKeyState = keyState;
    }

    private void ResolveCollision(Rectangle solidBounds, bool bounce)
    {
        var playerRect = GetBounds();
        var intersection = Rectangle.Intersect(playerRect, solidBounds);
        if (intersection.Width <= 0 || intersection.Height <= 0)
            return;

        float half = GameConstants.PlayerSize / 2f;

        // Determine the smallest separation axis to push out along
        if (intersection.Width < intersection.Height)
        {
            // Push out horizontally
            if (Position.X < solidBounds.Center.X)
                Position.X = solidBounds.Left - half;
            else
                Position.X = solidBounds.Right + half;

            if (bounce)
                Velocity.X = -Velocity.X * GameConstants.BounceRestitution;
            else
                Velocity.X = 0;
        }
        else
        {
            // Push out vertically
            if (Position.Y < solidBounds.Center.Y)
                Position.Y = solidBounds.Top - half;
            else
                Position.Y = solidBounds.Bottom + half;

            if (bounce)
                Velocity.Y = -Velocity.Y * GameConstants.BounceRestitution;
            else
                Velocity.Y = 0;
        }
    }

    private Rectangle GetBounds()
    {
        int size = GameConstants.PlayerSize;
        return new Rectangle(
            (int)(Position.X - size / 2),
            (int)(Position.Y - size / 2),
            size, size);
    }

    public Station? GetHoveredStation(List<Station> stations)
    {
        var mousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
        return stations
            .Where(s => s.Bounds.Contains(mousePoint) && s.DistanceTo(Position) <= GameConstants.InteractRange)
            .FirstOrDefault();
    }

    public void Draw(SpriteBatch spriteBatch, TextureManager textures)
    {
        int size = GameConstants.PlayerSize;
        var rect = GetBounds();

        // Tint the player when sprinting
        var color = IsSprinting ? Color.Orange : Color.CornflowerBlue;
        textures.DrawRect(spriteBatch, rect, color);

        if (HeldItem != null)
        {
            var itemRect = new Rectangle(
                (int)(Position.X - HeldItem.Size / 2),
                (int)(Position.Y - size / 2 - HeldItem.Size - 4),
                HeldItem.Size, HeldItem.Size);
            textures.DrawRect(spriteBatch, itemRect, HeldItem.DisplayColor);
            textures.DrawOutline(spriteBatch, itemRect, Color.Black, 1);
        }
    }
}
