using System;
using DxCore.Core.Components.Basic;
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

        private int TileWidth => (int) Math.Round(TileUnitWidth * Unit.Value);
        private int TileHeight => (int) Math.Round(TileUnitHeight * Unit.Value);

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

        public Rectangle TileForPoint(Point point)
        {
            // Probably works?
            Validate.Hard.IsInClosedInterval(point.X, 0, MapWidth * TileWidth);
            Validate.Hard.IsInClosedInterval(point.Y, 0, MapHeight * TileHeight);

            int x = point.X / TileWidth * TileWidth;
            int y = point.Y / TileHeight * TileHeight;

            return new Rectangle(x, y, TileWidth, TileHeight);
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
