#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class RiceCookerStation : Station
{
    private enum State { Idle, Cooking, Ready }
    private State _state = State.Idle;
    private float _timer;

    public override Color StationColor => new Color(180, 180, 180);
    public override string Label => "Rice Cooker";

    public RiceCookerStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem)
    {
        if (heldItem != null) return false;
        return _state == State.Idle || _state == State.Ready;
    }

    public override void Interact(ref Item? playerItem)
    {
        switch (_state)
        {
            case State.Idle when playerItem == null:
                _state = State.Cooking;
                _timer = GameConstants.RiceCookTime;
                break;
            case State.Ready when playerItem == null:
                playerItem = new Rice();
                _state = State.Idle;
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (_state == State.Cooking)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer <= 0)
                _state = State.Ready;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        base.Draw(spriteBatch, textures, font);

        if (_state == State.Cooking)
        {
            var barRect = new Rectangle(Bounds.X, Bounds.Bottom + 20, Bounds.Width, 8);
            float progress = 1f - _timer / GameConstants.RiceCookTime;
            textures.DrawProgressBar(spriteBatch, barRect, progress, Color.Yellow, new Color(40, 40, 40));
        }
        else if (_state == State.Ready)
        {
            var itemRect = new Rectangle(
                Bounds.Center.X - GameConstants.ItemSize / 2,
                Bounds.Center.Y - GameConstants.ItemSize / 2,
                GameConstants.ItemSize, GameConstants.ItemSize);
            textures.DrawRect(spriteBatch, itemRect, Color.White);
        }
    }
}
