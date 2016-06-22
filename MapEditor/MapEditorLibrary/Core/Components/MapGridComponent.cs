using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Components.Basic;
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

        public DxUnit Unit { get; }

        public MapGridComponent(DxUnit unit, int tileUnitWidth, int tileUnitHeight, int mapWidth, int mapHeight)
        {
            Validate.Hard.IsNotNullOrDefault(unit, () => this.GetFormattedNullOrDefaultMessage(unit));
            Unit = unit;
            Validate.Hard.IsPositive(tileUnitWidth, () => $"Cannot create a {GetType()} with {tileUnitWidth} unit width");
            TileUnitWidth = tileUnitWidth;
            Validate.Hard.IsPositive(tileUnitHeight, () => $"Cannot create a {GetType()} with {tileUnitHeight} unit height");
            TileUnitHeight = tileUnitHeight;
            Validate.Hard.IsPositive(mapWidth, () => $"Cannot create a {GetType()} with {mapWidth} map width");
            MapWidth = mapWidth;
            Validate.Hard.IsPositive(mapHeight, () => $"Cannot create a {GetType()} with {mapHeight} map height");
            MapHeight = mapHeight;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            int width = (int) Math.Round(TileUnitWidth * Unit.Value);
            int height = (int) Math.Round(TileUnitHeight * Unit.Value);
            for(int i = 0; i < MapWidth; ++i)
            {
                spriteBatch.DrawLine(new DxVector2(width * i, 0), new DxVector2(width * i, MapHeight * height),
                    Color.Red, 1, 0.9f);
            }
            for(int j = 0; j < MapHeight; ++j)
            {
                spriteBatch.DrawLine(new DxVector2(0, j * height), new DxVector2(MapWidth * width, j * height), Color.Red, 1,
                    0.9f);
            }
        }
    }
}
