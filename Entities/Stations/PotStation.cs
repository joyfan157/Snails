#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Systems;

namespace Snails.Entities.Stations;

public class PotStation : Station
{
    private enum State { Empty, HasFirst, Cooking, Ready }
    private State _state = State.Empty;
    private Item? _firstItem;
    private float _timer;
    private float _totalCookTime;
    private ItemType? _cookingOutput;

    public override Color StationColor => new Color(120, 100, 80);
    public override string Label => "Pot";

    public PotStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem)
    {
        // Deposit: pot must accept items
        if (heldItem != null)
            return _state == State.Empty || _state == State.HasFirst;
        // Pick up: pot must have something to give
        return _state == State.Ready || _state == State.HasFirst;
    }

    public override void Interact(ref Item? playerItem)
    {
        switch (_state)
        {
            case State.Empty:
                if (playerItem != null)
                {
                    _firstItem = playerItem;
                    _state = State.HasFirst;
                    playerItem = null;
                }
                break;

            case State.HasFirst:
                // Pick up the lone first item
                if (playerItem == null)
                {
                    playerItem = _firstItem;
                    _firstItem = null;
                    _state = State.Empty;
                    return;
                }

                // Check if second item completes a valid recipe
                var recipe = RecipeManager.Instance.FindCombiningRecipe(
                    _firstItem!.Type, playerItem.Type, "Pot");

                if (recipe != null)
                {
                    _firstItem = null;
                    _state = State.Cooking;
                    _totalCookTime = recipe.CookTime ?? GameConstants.PotCookTime;
                    _timer = _totalCookTime;
                    _cookingOutput = recipe.Output;
                    playerItem = null;
                }
                else
                {
                    // No valid recipe — swap player's item with stored one
                    var temp = _firstItem;
                    _firstItem = playerItem;
                    playerItem = temp;
                }
                break;

            case State.Ready:
                if (playerItem == null)
                {
                    playerItem = HeldItem;
                    HeldItem = null;
                    _state = State.Empty;
                }
                else
                {
                    // Swap with finished dish
                    var temp = HeldItem;
                    HeldItem = playerItem;
                    playerItem = temp;
                }
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (_state == State.Cooking)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer <= 0)
            {
                HeldItem = Item.Create(_cookingOutput);
                _cookingOutput = null;
                _state = State.Ready;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        base.Draw(spriteBatch, textures, font);

        if (_state == State.HasFirst && _firstItem != null)
        {
            var itemRect = new Rectangle(
                Bounds.Center.X - GameConstants.ItemSize / 2,
                Bounds.Center.Y - GameConstants.ItemSize / 2,
                GameConstants.ItemSize, GameConstants.ItemSize);
            textures.DrawRect(spriteBatch, itemRect, _firstItem.DisplayColor);
        }
        else if (_state == State.Cooking)
        {
            var barRect = new Rectangle(Bounds.X, Bounds.Bottom + 20, Bounds.Width, 8);
            float progress = 1f - _timer / _totalCookTime;
            textures.DrawProgressBar(spriteBatch, barRect, progress, Color.Orange, new Color(40, 40, 40));
        }
    }
}
