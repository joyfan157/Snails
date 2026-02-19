#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public abstract class Station
{
    public Vector2 Position { get; }
    public Rectangle Bounds { get; }
    public Item? HeldItem { get; protected set; }
    public abstract Color StationColor { get; }
    public abstract string Label { get; }

    protected Station(Vector2 position)
    {
        Position = position;
        Bounds = new Rectangle(
            (int)(position.X - GameConstants.StationSize / 2),
            (int)(position.Y - GameConstants.StationSize / 2),
            GameConstants.StationSize,
            GameConstants.StationSize);
    }

    public abstract void Interact(ref Item? playerItem);

    /// <summary>
    /// Returns true if an entity holding heldItem can meaningfully interact with this station.
    /// Used by both player (to gate clicks) and ghosts (to decide whether to wait).
    /// </summary>
    public virtual bool CanInteract(Item? heldItem) => true;

    /// <summary>
    /// Helper for source stations: gives the item to the caller only if their hand is empty.
    /// Returns true if the item was given.
    /// </summary>
    protected static bool TryGiveItem(ref Item? hand, Item item)
    {
        if (hand != null) return false;
        hand = item;
        return true;
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        textures.DrawRect(spriteBatch, Bounds, StationColor);
        textures.DrawOutline(spriteBatch, Bounds, Color.Black, 2);

        if (HeldItem != null)
        {
            var itemRect = new Rectangle(
                Bounds.Center.X - HeldItem.Size / 2,
                Bounds.Center.Y - HeldItem.Size / 2,
                HeldItem.Size, HeldItem.Size);
            textures.DrawRect(spriteBatch, itemRect, HeldItem.DisplayColor);
        }

        var textSize = font.MeasureString(Label);
        var textPos = new Vector2(
            Bounds.Center.X - textSize.X / 2,
            Bounds.Bottom + 4);
        spriteBatch.DrawString(font, Label, textPos, Color.White);
    }

    public float DistanceTo(Vector2 point)
    {
        return Vector2.Distance(Position, point);
    }
}
