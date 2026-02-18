using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities;
using Snails.Entities.Items;
using Snails.Systems;

namespace Snails.UI;

public class HudRenderer
{
    private readonly OrderManager _orderManager;
    private readonly ScoreManager _scoreManager;

    public HudRenderer(OrderManager orderManager, ScoreManager scoreManager)
    {
        _orderManager = orderManager;
        _scoreManager = scoreManager;
    }

    public void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font, Player player)
    {
        // Dark HUD bar background
        textures.DrawRect(spriteBatch,
            new Rectangle(0, 0, GameConstants.WindowWidth, GameConstants.HudHeight),
            new Color(30, 30, 40));

        // Score
        string scoreText = $"Score: {_scoreManager.Score}";
        spriteBatch.DrawString(font, scoreText, new Vector2(10, 10), Color.White);

        // Held item indicator
        string heldText = player.HeldItem != null ? $"Holding: {player.HeldItem.DisplayName}" : "Hands empty";
        spriteBatch.DrawString(font, heldText, new Vector2(10, 35), Color.LightGray);

        // Order boxes
        int orderX = 250;
        for (int i = 0; i < _orderManager.ActiveOrders.Count; i++)
        {
            var order = _orderManager.ActiveOrders[i];
            DrawOrderBox(spriteBatch, textures, font, order, orderX + i * 90, 10);
        }
    }

    private void DrawOrderBox(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font,
        Order order, int x, int y)
    {
        var boxRect = new Rectangle(x, y, 80, 75);
        textures.DrawRect(spriteBatch, boxRect, new Color(50, 50, 60));
        textures.DrawOutline(spriteBatch, boxRect, Color.Gray, 1);

        // Item color indicator
        var itemColor = GetItemColor(order.RequestedItem);
        var itemRect = new Rectangle(x + 30, y + 5, 20, 20);
        textures.DrawRect(spriteBatch, itemRect, itemColor);

        // Label
        string label = GetDishLabel(order.RequestedItem);
        var labelSize = font.MeasureString(label);
        spriteBatch.DrawString(font, label, new Vector2(x + 40 - labelSize.X / 2, y + 28), Color.White);

        // Timer bar
        float timerProgress = order.TimeRemaining / order.TotalTime;
        var timerColor = timerProgress > 0.3f ? Color.Green : Color.Red;
        var barRect = new Rectangle(x + 5, y + 50, 70, 8);
        textures.DrawProgressBar(spriteBatch, barRect, timerProgress, timerColor, new Color(40, 40, 40));

        // Time remaining text
        string timeText = $"{(int)order.TimeRemaining}s";
        var timeSize = font.MeasureString(timeText);
        spriteBatch.DrawString(font, timeText, new Vector2(x + 40 - timeSize.X / 2, y + 60), Color.LightGray);
    }

    private static string GetDishLabel(ItemType type) => type switch
    {
        ItemType.Nigiri => "Nigiri",
        ItemType.MakiRoll => "Maki",
        ItemType.MisoSoup => "Miso",
        _ => "???"
    };

    private static Color GetItemColor(ItemType type) => type switch
    {
        ItemType.Nigiri => new Color(255, 200, 150),
        ItemType.Rice => Color.White,
        ItemType.Salmon => new Color(255, 150, 150),
        ItemType.ChoppedSalmon => new Color(200, 80, 80),
        ItemType.Nori => new Color(50, 80, 50),
        ItemType.Tofu => new Color(245, 235, 200),
        ItemType.ChoppedTofu => new Color(220, 210, 170),
        ItemType.Dashi => new Color(180, 160, 120),
        ItemType.MakiRoll => new Color(100, 180, 100),
        ItemType.MisoSoup => new Color(200, 170, 80),
        _ => Color.Gray
    };
}
