using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditorLibrary.Core.Components
{
    public class MapGridComponent : DrawableComponent
    {
        public int TileUnitWidth { get; }
        public int TileUnitHeight { get; }
        public int MapWidth { get; }
        public int MapHeight { get; }

        public Color Color { get; set; } = Color.White;

        public DxUnit Unit { get; }

        public int TileWidth => (int) Math.Round(TileUnitWidth * Unit.Value);
        public int TileHeight => (int) Math.Round(TileUnitHeight * Unit.Value);

        public DxRectangle Bounds => new DxRectangle(0, 0, TileWidth * MapWidth, TileHeight * MapHeight);

        public MapGridComponent(DxUnit unit, int tileUnitWidth, int tileUnitHeight, int mapWidth, int mapHeight)
        {
            Validate.Hard.IsNotNullOrDefault(unit, () => this.GetFormattedNullOrDefaultMessage(unit));
            Unit = unit;
            Validate.Hard.IsPositive(tileUnitWidth, () => $"Cannot create a {GetType()} with {tileUnitWidth} unit width");
            TileUnitWidth = tileUnitWidth;
            Validate.Hard.IsPositive(tileUnitHeight,
                () => $"Cannot create a {GetType()} with {tileUnitHeight} unit height");
            TileUnitHeight = tileUnitHeight;
            Validate.Hard.IsPositive(mapWidth, () => $"Cannot create a {GetType()} with {mapWidth} map width");
            MapWidth = mapWidth;
            Validate.Hard.IsPositive(mapHeight, () => $"Cannot create a {GetType()} with {mapHeight} map height");
            MapHeight = mapHeight;

            new UpdateCameraBounds(Bounds).Emit();
        }

        public bool PositionForPoint(DxVector2 point, out TilePosition tilePosition)
        {
            // Probably works?
            if(!Validate.Check.IsInClosedInterval(point.X, 0, MapWidth * TileWidth))
            {
                tilePosition = default(TilePosition);
                return false;
            }
            if(!Validate.Check.IsInClosedInterval(point.Y, 0, MapHeight * TileHeight))
            {
                tilePosition = default(TilePosition);
                return false;
            }

            int x = (int)Math.Round(point.X) / TileWidth;
            int y = (int)Math.Round(point.Y) / TileHeight;

            tilePosition =  new TilePosition(x, y);
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            for(int i = 0; i <= MapWidth; ++i)
            {
                spriteBatch.DrawLine(new DxVector2(TileWidth * i, 0),
                    new DxVector2(TileWidth * i, MapHeight * TileHeight), Color, 1, 0.9f);
            }
            for(int j = 0; j <= MapHeight; ++j)
            {
                spriteBatch.DrawLine(new DxVector2(0, j * TileHeight),
                    new DxVector2(MapWidth * TileWidth, j * TileHeight), Color, 1, 0.9f);
            }
        }
    }
}
