using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Snails.Core;
using Snails.Data;
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
    private LevelMenu _levelMenu;

    private LevelData _currentLevel;
    private int _currentLevelNumber = 1;

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

        _levelMenu = new LevelMenu();
        LoadLevel(_currentLevelNumber);
    }

    private void LoadLevel(int levelNumber)
    {
        // Load level data from JSON
        _currentLevel = LevelLoader.LoadLevel(levelNumber);

        // Initialize systems with level parameters
        _orderManager = new OrderManager(
            _currentLevel.AllowedDishes,
            _currentLevel.OrderSpawnMin,
            _currentLevel.OrderSpawnMax
        );
        _scoreManager = new ScoreManager();

        // Create stations from level data
        _stations = new List<Station>();
        foreach (var stationData in _currentLevel.Stations)
        {
            Vector2 position = new Vector2(stationData.X, stationData.Y);
            Station station = StationFactory.CreateStation(
                stationData.Type,
                position,
                _orderManager,
                _scoreManager
            );
            _stations.Add(station);
        }

        // Initialize obstacles (empty for now)
        _obstacles = new List<Obstacle>();

        // Create player at level-defined start position
        _player = new Player(new Vector2(_currentLevel.PlayerStartX, _currentLevel.PlayerStartY));
        _hud = new HudRenderer(_orderManager, _scoreManager);

        // Initialize ghost recording system
        _recorder = new GhostRecorder();
        _ghosts = new List<GhostEntity>();
        _player.OnStationInteraction += _recorder.RecordInteraction;
    }

    protected override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Escape))
            Exit();

        // Update level menu and check for level selection
        int? selectedLevel = _levelMenu.Update(keyState);
        if (selectedLevel.HasValue && selectedLevel.Value != _currentLevelNumber)
        {
            _currentLevelNumber = selectedLevel.Value;
            LoadLevel(_currentLevelNumber);
            return; // Skip rest of update on level change
        }

        // Pause game when menu is open
        if (_levelMenu.IsOpen)
        {
            base.Update(gameTime);
            return;
        }

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
            _ghosts.Count,
            _currentLevel.Name);

        // Draw level menu on top of everything
        _levelMenu.Draw(_spriteBatch, _textures, _font);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
