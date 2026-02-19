#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;
using Snails.Systems;

namespace Snails.Entities.Stations;

public class CuttingBoardStation : Station
{
    private Item? _firstItem;

    public override Color StationColor => new Color(210, 180, 140);
    public override string Label => "Board";

    public CuttingBoardStation(Vector2 position) : base(position) { }

    public override bool CanInteract(Item? heldItem)
    {
        // Pick up: something must be on the board
        if (heldItem == null)
            return HeldItem != null || _firstItem != null;
        // Deposit: board must have room (empty or has first item awaiting second)
        return HeldItem == null;
    }

    public override void Interact(ref Item? playerItem)
    {
        // Pick up completed dish or lone first item
        if (playerItem == null && HeldItem != null)
        {
            playerItem = HeldItem;
            HeldItem = null;
            return;
        }

        if (playerItem == null && _firstItem != null)
        {
            playerItem = _firstItem;
            _firstItem = null;
            return;
        }

        if (playerItem == null)
            return;

        // Place first item on empty board
        if (_firstItem == null && HeldItem == null)
        {
            _firstItem = playerItem;
            playerItem = null;
            return;
        }

        // Place second item — combine if valid recipe, otherwise swap
        if (_firstItem != null && HeldItem == null)
        {
            var recipe = RecipeManager.Instance.FindCombiningRecipe(
                _firstItem.Type, playerItem.Type, "CuttingBoard");
            if (recipe != null)
            {
                _firstItem = null;
                HeldItem = Item.Create(recipe.Output);
                playerItem = null;
            }
            else
            {
                // No valid recipe — swap player's item with the stored one
                var temp = _firstItem;
                _firstItem = playerItem;
                playerItem = temp;
            }
            return;
        }

        // Swap with completed result
        if (HeldItem != null)
        {
            var temp = HeldItem;
            HeldItem = playerItem;
            playerItem = temp;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, TextureManager textures, SpriteFont font)
    {
        base.Draw(spriteBatch, textures, font);

        if (_firstItem != null && HeldItem == null)
        {
            var itemRect = new Rectangle(
                Bounds.Center.X - GameConstants.ItemSize / 2,
                Bounds.Center.Y - GameConstants.ItemSize / 2,
                GameConstants.ItemSize, GameConstants.ItemSize);
            textures.DrawRect(spriteBatch, itemRect, _firstItem.DisplayColor);
        }
    }
}
