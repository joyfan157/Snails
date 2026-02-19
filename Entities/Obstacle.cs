using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;

namespace Snails.Entities;

public class Obstacle
{
    public Vector2 Position { get; }
    public Rectangle Bounds { get; }
    public Color DisplayColor { get; }
    public string Label { get; }

    public Obstacle(Vector2 position, Color color, string label)
    {
        Position = position;
        DisplayColor = color;
        Label = label;
        int size = GameConstants.ObstacleSize;
        Bounds = new Rectangle(
            (int)(position.X - size / 2),
            (int)(position.Y - size / 2),
            size, size);
    }

    public void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        textures.DrawRect(spriteBatch, Bounds, DisplayColor);
        textures.DrawOutline(spriteBatch, Bounds, Color.Black, 2);

        var textSize = font.MeasureString(Label);
        var textPos = new Vector2(
            Bounds.Center.X - textSize.X / 2,
            Bounds.Bottom + 4);
        spriteBatch.DrawString(font, Label, textPos, Color.LightGray);
    }
}
