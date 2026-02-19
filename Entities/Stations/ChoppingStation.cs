#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Systems;

namespace Snails.Entities.Stations;

public class ChoppingStation : Station
{
    private enum State { Idle, Chopping, Ready }
    private State _state = State.Idle;
    private float _timer;
    private float _totalChopTime;
    private Item? _result;

    public override Color StationColor => new Color(139, 90, 43);
    public override string Label => "Chopping";

    public ChoppingStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem)
    {
        // Can deposit a choppable item (or any item to store) when idle
        if (heldItem != null)
            return _state == State.Idle;
        // Can pick up when ready or when station holds a stored item
        return _state == State.Ready || (_state == State.Idle && HeldItem != null);
    }

    public override void Interact(ref Item? playerItem)
    {
        switch (_state)
        {
            case State.Idle:
                // Pick up stored item
                if (playerItem == null && HeldItem != null)
                {
                    playerItem = HeldItem;
                    HeldItem = null;
                    return;
                }

                // Swap with stored item
                if (playerItem != null && HeldItem != null)
                {
                    var temp = HeldItem;
                    HeldItem = playerItem;
                    playerItem = temp;
                    TryStartChopping();
                    return;
                }

                // Choppable items start processing
                if (playerItem != null && playerItem.IsChoppable)
                {
                    StartChopping(playerItem);
                    playerItem = null;
                    return;
                }

                // Store any other item
                if (playerItem != null)
                {
                    HeldItem = playerItem;
                    playerItem = null;
                }
                break;

            case State.Ready:
                if (playerItem == null)
                {
                    playerItem = _result;
                    _result = null;
                    _state = State.Idle;
                }
                else
                {
                    // Swap: take result, leave player's item
                    var result = _result;
                    _result = null;
                    _state = State.Idle;

                    if (playerItem.IsChoppable)
                    {
                        StartChopping(playerItem);
                    }
                    else
                    {
                        HeldItem = playerItem;
                    }
                    playerItem = result;
                }
                break;
        }
    }

    private void StartChopping(Item item)
    {
        var recipe = RecipeManager.Instance.GetChoppingRecipe(item.Type);
        if (recipe == null) return;
        _result = Item.Create(recipe.Output);
        _state = State.Chopping;
        _totalChopTime = recipe.CookTime;
        _timer = _totalChopTime;
    }

    private void TryStartChopping()
    {
        if (HeldItem != null && HeldItem.IsChoppable)
        {
            StartChopping(HeldItem);
            HeldItem = null;
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
            float progress = 1f - _timer / _totalChopTime;
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
