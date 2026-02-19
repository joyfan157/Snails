#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snails.UI;

public enum MenuTab { Levels, Instructions }

public class LevelMenu
{
    public bool IsOpen { get; private set; }
    private KeyboardState _prevKeyState;
    private List<int> _availableLevels;
    private MenuTab _currentTab = MenuTab.Levels;

    public LevelMenu()
    {
        _availableLevels = ScanAvailableLevels();
    }

    private List<int> ScanAvailableLevels()
    {
        var levels = new List<int>();
        var levelsDir = Path.Combine("Data", "Levels");

        if (!Directory.Exists(levelsDir))
            return levels;

        var files = Directory.GetFiles(levelsDir, "level*.json");
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName.StartsWith("level") && int.TryParse(fileName.Substring(5), out int levelNum))
            {
                levels.Add(levelNum);
            }
        }

        levels.Sort();
        return levels;
    }

    public int? Update(KeyboardState keyState)
    {
        // Toggle menu with Tab key
        if (keyState.IsKeyDown(Keys.Tab) && _prevKeyState.IsKeyUp(Keys.Tab))
        {
            IsOpen = !IsOpen;
            if (IsOpen)
                _currentTab = MenuTab.Levels; // Reset to levels tab when opening
        }

        int? selectedLevel = null;

        // If menu is open, check for navigation and selection
        if (IsOpen)
        {
            // Switch tabs with arrow keys
            if (keyState.IsKeyDown(Keys.Left) && _prevKeyState.IsKeyUp(Keys.Left))
            {
                _currentTab = MenuTab.Levels;
            }
            if (keyState.IsKeyDown(Keys.Right) && _prevKeyState.IsKeyUp(Keys.Right))
            {
                _currentTab = MenuTab.Instructions;
            }

            // Only check for level selection when on Levels tab
            if (_currentTab == MenuTab.Levels)
            {
                foreach (var level in _availableLevels)
                {
                    if (level >= 1 && level <= 9)
                    {
                        var numberKey = Keys.D0 + level; // D1, D2, D3, etc.
                        if (keyState.IsKeyDown(numberKey) && _prevKeyState.IsKeyUp(numberKey))
                        {
                            selectedLevel = level;
                            IsOpen = false;
                            break;
                        }
                    }
                }
            }
        }

        _prevKeyState = keyState;
        return selectedLevel;
    }

    public void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        if (!IsOpen)
            return;

        // Semi-transparent overlay
        var overlay = new Rectangle(0, 0, GameConstants.WindowWidth, GameConstants.WindowHeight);
        textures.DrawRect(spriteBatch, overlay, new Color(0, 0, 0, 180));

        // Menu box
        int menuWidth = 600;
        int menuHeight = 500;
        var menuBox = new Rectangle(
            GameConstants.WindowWidth / 2 - menuWidth / 2,
            GameConstants.WindowHeight / 2 - menuHeight / 2,
            menuWidth,
            menuHeight
        );
        textures.DrawRect(spriteBatch, menuBox, new Color(40, 40, 50));
        textures.DrawOutline(spriteBatch, menuBox, Color.White, 3);

        // Tab buttons
        int tabWidth = 150;
        int tabHeight = 35;
        int tabY = menuBox.Y + 10;
        int tabSpacing = 10;

        var levelsTabRect = new Rectangle(menuBox.X + 20, tabY, tabWidth, tabHeight);
        var instructionsTabRect = new Rectangle(menuBox.X + 20 + tabWidth + tabSpacing, tabY, tabWidth, tabHeight);

        // Draw tabs
        DrawTab(spriteBatch, textures, font, levelsTabRect, "LEVELS", _currentTab == MenuTab.Levels);
        DrawTab(spriteBatch, textures, font, instructionsTabRect, "INSTRUCTIONS", _currentTab == MenuTab.Instructions);

        // Content area
        int contentY = menuBox.Y + tabHeight + 30;

        if (_currentTab == MenuTab.Levels)
        {
            DrawLevelsContent(spriteBatch, textures, font, menuBox, contentY);
        }
        else
        {
            DrawInstructionsContent(spriteBatch, font, menuBox, contentY);
        }

        // Bottom instructions
        string navText = "Left/Right: Switch Tabs  |  TAB: Close";
        var navSize = font.MeasureString(navText);
        var navPos = new Vector2(
            GameConstants.WindowWidth / 2 - navSize.X / 2,
            menuBox.Bottom - 30
        );
        spriteBatch.DrawString(font, navText, navPos, new Color(150, 150, 150));
    }

    private void DrawTab(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font,
        Rectangle tabRect, string text, bool isActive)
    {
        var tabColor = isActive ? new Color(60, 60, 80) : new Color(30, 30, 40);
        var textColor = isActive ? Color.White : Color.Gray;
        var borderColor = isActive ? Color.White : Color.DarkGray;

        textures.DrawRect(spriteBatch, tabRect, tabColor);
        textures.DrawOutline(spriteBatch, tabRect, borderColor, 2);

        var textSize = font.MeasureString(text);
        var textPos = new Vector2(
            tabRect.X + tabRect.Width / 2 - textSize.X / 2,
            tabRect.Y + tabRect.Height / 2 - textSize.Y / 2
        );
        spriteBatch.DrawString(font, text, textPos, textColor);
    }

    private void DrawLevelsContent(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font,
        Rectangle menuBox, int contentY)
    {
        string title = "SELECT LEVEL";
        var titleSize = font.MeasureString(title);
        var titlePos = new Vector2(
            GameConstants.WindowWidth / 2 - titleSize.X / 2,
            contentY
        );
        spriteBatch.DrawString(font, title, titlePos, Color.White);

        // Level list
        int yOffset = contentY + 50;
        foreach (var level in _availableLevels)
        {
            string levelText = $"Press {level} - Level {level}";
            var textSize = font.MeasureString(levelText);
            var textPos = new Vector2(
                GameConstants.WindowWidth / 2 - textSize.X / 2,
                yOffset
            );
            spriteBatch.DrawString(font, levelText, textPos, Color.LightGray);
            yOffset += 40;
        }
    }

    private void DrawInstructionsContent(SpriteBatch spriteBatch, SpriteFont font,
        Rectangle menuBox, int contentY)
    {
        string title = "HOW TO PLAY";
        var titleSize = font.MeasureString(title);
        var titlePos = new Vector2(
            GameConstants.WindowWidth / 2 - titleSize.X / 2,
            contentY
        );
        spriteBatch.DrawString(font, title, titlePos, Color.White);

        // Instructions text
        var instructions = new[]
        {
            "MOVEMENT:",
            "  WASD - Move",
            "  Hold Shift - Sprint (uses stamina)",
            "",
            "COOKING:",
            "  Click stations to interact",
            "  Pick up ingredients, process them,",
            "  and deliver to Output station",
            "",
            "GHOST SYSTEM:",
            "  Space - Start/Stop recording",
            "  Recordings spawn helper ghosts",
            "  Ghosts replay your actions in a loop",
            "",
            "GOAL:",
            "  Fulfill orders before time runs out!",
            "  Q - Trash held item"
        };

        int yOffset = contentY + 50;
        int leftMargin = menuBox.X + 40;

        foreach (var line in instructions)
        {
            var color = line.EndsWith(":") ? Color.Yellow : Color.LightGray;
            spriteBatch.DrawString(font, line, new Vector2(leftMargin, yOffset), color);
            yOffset += 24;
        }
    }
}
