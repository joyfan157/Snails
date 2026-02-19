using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Entities;
using Snails.Entities.Ghost;
using Snails.Entities.Items;
using Snails.Entities.Stations;
using Snails.Systems;
using Snails.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace Snails;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private TextureManager _textures;
    private SpriteFont _font;

    private Player _player;
    private List<Station> _stations;
    private List<Obstacle> _obstacles;
    private OrderManager _orderManager;
    private ScoreManager _scoreManager;
    private HudRenderer _hud;
    private GhostRecorder _recorder;
    private List<GhostEntity> _ghosts;

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

        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        Item.LoadDefinitions(Path.Combine(dataDir, "items.json"));
        RecipeManager.Initialize(Path.Combine(dataDir, "recipes.json"));

        _orderManager = new OrderManager();
        _scoreManager = new ScoreManager();

        // Kitchen layout: spread across a large kitchen with multiple workstations
        _stations = new List<Station>
        {
            // Ingredient sources — scattered along the left and bottom
            new RiceCookerStation(new Vector2(120, 220)),
            new SalmonStation(new Vector2(120, 440)),
            new NoriStation(new Vector2(300, 220)),
            new TofuStation(new Vector2(120, 660)),
            new DashiStation(new Vector2(300, 760)),

            // Chopping stations — two in the center area
            new ChoppingStation(new Vector2(500, 300)),
            new ChoppingStation(new Vector2(500, 580)),

            // Cutting boards — two on the right side
            new CuttingBoardStation(new Vector2(820, 220)),
            new CuttingBoardStation(new Vector2(1060, 440)),

            // Pots — two spread apart
            new PotStation(new Vector2(820, 580)),
            new PotStation(new Vector2(1060, 720)),

            // Output — serve window at far right
            new OutputStation(new Vector2(1160, 220), _orderManager, _scoreManager)
        };

        _obstacles = new List<Obstacle>();

        _player = new Player(new Vector2(600, 450));
        _hud = new HudRenderer(_orderManager, _scoreManager);

        _recorder = new GhostRecorder();
        _ghosts = new List<GhostEntity>();
        _player.OnStationInteraction += _recorder.RecordInteraction;
    }

    protected override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Escape))
            Exit();

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Check recording toggle (Space key)
        var completedRecording = _recorder.Update(keyState, _player);

        _player.Update(gameTime, _stations, _obstacles);

        // Capture frame after player update for accurate positions
        _recorder.CaptureFrame(_player, dt);

        // Spawn ghost from completed recording
        if (completedRecording != null && completedRecording.Frames.Count > 0)
        {
            var color = GameConstants.GhostColors[_ghosts.Count % GameConstants.GhostColors.Length];
            _ghosts.Add(new GhostEntity(completedRecording, color, _stations));
        }

        // Update all ghosts
        foreach (var ghost in _ghosts)
            ghost.Update(gameTime);

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

        // Draw obstacles
        foreach (var obstacle in _obstacles)
            obstacle.Draw(_spriteBatch, _textures, _font);

        // Draw ghosts (before player so player renders on top)
        foreach (var ghost in _ghosts)
            ghost.Draw(_spriteBatch, _textures, _font);

        // Highlight hovered station within range
        var hovered = _player.GetHoveredStation(_stations);
        if (hovered != null)
            _textures.DrawOutline(_spriteBatch, hovered.Bounds, Color.White, 3);

        // Draw player
        _player.Draw(_spriteBatch, _textures);

        // Draw HUD
        _hud.Draw(_spriteBatch, _textures, _font, _player,
            _recorder.State == RecordingState.Recording,
            _recorder.RecordingTimer,
            _ghosts.Count);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
