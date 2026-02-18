using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Entities;
using Snails.Entities.Stations;
using Snails.Systems;
using Snails.UI;
using System.Collections.Generic;

namespace Snails;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private TextureManager _textures;
    private SpriteFont _font;

    private Player _player;
    private List<Station> _stations;
    private OrderManager _orderManager;
    private ScoreManager _scoreManager;
    private HudRenderer _hud;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = GameConstants.WindowWidth;
        _graphics.PreferredBackBufferHeight = GameConstants.WindowHeight;
        _graphics.ApplyChanges();
        Window.Title = "Nigiri Sushi";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _textures = new TextureManager(GraphicsDevice);
        _font = Content.Load<SpriteFont>("DefaultFont");

        _orderManager = new OrderManager();
        _scoreManager = new ScoreManager();

        // Kitchen layout: ingredients left, processing center, assembly/output right
        _stations = new List<Station>
        {
            // Left column — ingredient sources
            new RiceCookerStation(new Vector2(120, 220)),
            new SalmonStation(new Vector2(120, 400)),
            new NoriStation(new Vector2(280, 220)),
            new TofuStation(new Vector2(280, 400)),
            new DashiStation(new Vector2(280, 560)),

            // Center — processing
            new ChoppingStation(new Vector2(480, 310)),

            // Right column — assembly & output
            new CuttingBoardStation(new Vector2(780, 220)),
            new PotStation(new Vector2(780, 400)),
            new OutputStation(new Vector2(780, 560), _orderManager, _scoreManager)
        };

        _player = new Player(new Vector2(480, 450));
        _hud = new HudRenderer(_orderManager, _scoreManager);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _player.Update(gameTime, _stations);
        foreach (var station in _stations)
            station.Update(gameTime);
        _orderManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(60, 60, 80));

        _spriteBatch.Begin();

        // Draw stations
        foreach (var station in _stations)
            station.Draw(_spriteBatch, _textures, _font);

        // Highlight hovered station within range
        var hovered = _player.GetHoveredStation(_stations);
        if (hovered != null)
            _textures.DrawOutline(_spriteBatch, hovered.Bounds, Color.White, 3);

        // Draw player
        _player.Draw(_spriteBatch, _textures);

        // Draw HUD
        _hud.Draw(_spriteBatch, _textures, _font, _player);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
