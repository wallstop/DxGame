using System.Collections.Generic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Utils
{
    public static class SpriteBatchUtils
    {
        public static void DrawBorder(DxGame game, DxRectangle rectangle, int borderThickness, Color borderColor)
        {
            DrawBorder(game, rectangle.ToRectangle(), borderThickness, borderColor);
        }

        public static void DrawBorder(DxGame game, Rectangle rectangle, int borderThickness, Color borderColor)
        {
            var coloredPixel = TextureFactory.TextureForColor(borderColor);
            var spriteBatch = game.SpriteBatch;
            IEnumerable<Rectangle> borderRectangles = GenerateBorderRectangles(rectangle, borderThickness);
            foreach (Rectangle borderRectangle in borderRectangles)
            {
                spriteBatch.Draw(coloredPixel, destinationRectangle: borderRectangle);
            }
        }

        private static IEnumerable<Rectangle> GenerateBorderRectangles(Rectangle rectangle, int borderThickness)
        {
            int numRectangles = 4;
            Rectangle[] generatedBorderRectangles = new Rectangle[numRectangles];
            for (int i = 0; i < numRectangles; ++i)
            {
                Rectangle copy = rectangle;
                switch (i)
                {
                    /* Left edge */
                    case 0:
                        copy.Width = borderThickness;
                        break;
                    /* Top edge */
                    case 1:
                        copy.Height = borderThickness;
                        break;
                    /* Right edge */
                    case 2:
                        copy.X = copy.X + copy.Width - borderThickness;
                        copy.Width = borderThickness;
                        break;
                    /* Bottom edge */
                    case 3:
                        copy.Y = copy.Y + copy.Height - borderThickness;
                        copy.Height = borderThickness;
                        break;
                }
                generatedBorderRectangles[i] = copy;
            }
            return generatedBorderRectangles;
        }
    }
}