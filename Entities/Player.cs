#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Entities.Stations;
using System.Collections.Generic;
using System.Linq;

namespace Snails.Entities;

public class Player
{
    public Vector2 Position;
    public Item? HeldItem;

    private MouseState _prevMouseState;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void Update(GameTime gameTime, List<Station> stations)
    {
        var keyState = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float speed = GameConstants.PlayerSpeed;

        var move = Vector2.Zero;
        if (keyState.IsKeyDown(Keys.W)) move.Y -= 1;
        if (keyState.IsKeyDown(Keys.S)) move.Y += 1;
        if (keyState.IsKeyDown(Keys.A)) move.X -= 1;
        if (keyState.IsKeyDown(Keys.D)) move.X += 1;

        if (move != Vector2.Zero)
        {
            move.Normalize();
            Position += move * speed * dt;
        }

        float half = GameConstants.PlayerSize / 2f;
        Position.X = MathHelper.Clamp(Position.X, half, GameConstants.WindowWidth - half);
        Position.Y = MathHelper.Clamp(Position.Y, GameConstants.HudHeight + half, GameConstants.WindowHeight - half);

        // Click interaction: must click a station within interact range
        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed &&
            _prevMouseState.LeftButton == ButtonState.Released)
        {
            var mousePoint = new Point(mouseState.X, mouseState.Y);
            var clicked = stations
                .Where(s => s.Bounds.Contains(mousePoint) && s.DistanceTo(Position) <= GameConstants.InteractRange)
                .FirstOrDefault();

            if (clicked != null)
            {
                var item = HeldItem;
                clicked.Interact(ref item);
                HeldItem = item;
            }
        }

        _prevMouseState = mouseState;
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
        var rect = new Rectangle(
            (int)(Position.X - size / 2),
            (int)(Position.Y - size / 2),
            size, size);
        textures.DrawRect(spriteBatch, rect, Color.CornflowerBlue);

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
