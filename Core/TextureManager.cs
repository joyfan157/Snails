using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Snails.Core;

public class TextureManager
{
    public Texture2D Pixel { get; }

    public TextureManager(GraphicsDevice graphicsDevice)
    {
        Pixel = new Texture2D(graphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }

    public void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        spriteBatch.Draw(Pixel, rect, color);
    }

    public void DrawOutline(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 2)
    {
        // Top
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(Pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
    }

    public void DrawProgressBar(SpriteBatch spriteBatch, Rectangle rect, float progress, Color fillColor, Color bgColor)
    {
        DrawRect(spriteBatch, rect, bgColor);
        var fillWidth = (int)(rect.Width * MathHelper.Clamp(progress, 0f, 1f));
        if (fillWidth > 0)
            DrawRect(spriteBatch, new Rectangle(rect.X, rect.Y, fillWidth, rect.Height), fillColor);
    }
}
