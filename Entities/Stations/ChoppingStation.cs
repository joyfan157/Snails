#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class ChoppingStation : Station
{
    private enum State { Idle, Chopping, Ready }
    private State _state = State.Idle;
    private float _timer;
    private Item? _result;

    public override Color StationColor => new Color(139, 90, 43);
    public override string Label => "Chopping";

    public ChoppingStation(Vector2 position) : base(position) { }

    public override void Interact(ref Item? playerItem)
    {
        switch (_state)
        {
            case State.Idle when playerItem is Salmon:
                _state = State.Chopping;
                _timer = GameConstants.ChopTime;
                _result = new ChoppedSalmon();
                playerItem = null;
                break;
            case State.Idle when playerItem is Tofu:
                _state = State.Chopping;
                _timer = GameConstants.ChopTime;
                _result = new ChoppedTofu();
                playerItem = null;
                break;
            case State.Ready:
                if (playerItem == null)
                {
                    playerItem = _result;
                    _result = null;
                    _state = State.Idle;
                }
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (_state == State.Chopping)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer <= 0)
                _state = State.Ready;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        base.Draw(spriteBatch, textures, font);

        if (_state == State.Chopping)
        {
            var barRect = new Rectangle(Bounds.X, Bounds.Bottom + 20, Bounds.Width, 8);
            float progress = 1f - _timer / GameConstants.ChopTime;
            textures.DrawProgressBar(spriteBatch, barRect, progress, Color.Yellow, new Color(40, 40, 40));
        }
        else if (_state == State.Ready && _result != null)
        {
            var itemRect = new Rectangle(
                Bounds.Center.X - GameConstants.ItemSize / 2,
                Bounds.Center.Y - GameConstants.ItemSize / 2,
                GameConstants.ItemSize, GameConstants.ItemSize);
            textures.DrawRect(spriteBatch, itemRect, _result.DisplayColor);
        }
    }
}
