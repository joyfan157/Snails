#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snails.Core;
using Snails.Entities.Items;

namespace Snails.Entities.Stations;

public class CuttingBoardStation : Station
{
    private Item? _firstItem;

    public override Color StationColor => new Color(210, 180, 140);
    public override string Label => "Board";

    public CuttingBoardStation(Vector2 position) : base(position) { }

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

        // Only accept valid board ingredients
        if (playerItem is not (Rice or ChoppedSalmon or Nori))
            return;

        // Place first item on empty board
        if (_firstItem == null && HeldItem == null)
        {
            _firstItem = playerItem;
            playerItem = null;
            return;
        }

        // Place second item — check for valid recipes
        if (_firstItem != null && HeldItem == null)
        {
            Item? result = GetRecipeResult(_firstItem, playerItem);
            if (result != null)
            {
                _firstItem = null;
                HeldItem = result;
                playerItem = null;
            }
        }
    }

    private static Item? GetRecipeResult(Item first, Item second)
    {
        // Nigiri: Rice + ChoppedSalmon (any order)
        if ((first is Rice && second is ChoppedSalmon) ||
            (first is ChoppedSalmon && second is Rice))
            return new Nigiri();

        // Maki Roll: Rice + Nori (any order)
        if ((first is Rice && second is Nori) ||
            (first is Nori && second is Rice))
            return new MakiRoll();

        return null;
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
