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

        /* A digital frontier. I tried to picture clusters of information as they moved through the computer... what did they look like? Chips? Motorcycles? Were the circuits like freeways? I kept dreaming of a world I thought I'd never see. And then, one day, I got in. */
        private List<Rectangle> TheGrid { get; set; } = new List<Rectangle>();

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

            RegenerateGrid();
        }

        // TODO: If we want, we can make the properties mutatable and regenerate the grid every time they mutate
        private void RegenerateGrid()
        {
            TheGrid = new List<Rectangle>(MapWidth * MapHeight);
            int width = (int )Math.Round(TileUnitWidth * Unit.Value);
            int height = (int) Math.Round(TileUnitHeight * Unit.Value);
            for(int i = 0; i < MapWidth; ++i)
            {
                for(int j = 0; j < MapHeight; ++j)
                {
                    Rectangle rectangle = new Rectangle(i * width, j * height, width, height);
                    TheGrid.Add(rectangle);
                }
            }
        }


        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach(Rectangle tile in TheGrid)
            {
                spriteBatch.DrawBorder(tile, 1, Color.Red);
            }
        }
    }
}
