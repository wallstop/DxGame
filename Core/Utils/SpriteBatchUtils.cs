using System.Collections.Generic;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Utils
{
    public static class SpriteBatchUtils
    {
        public static void DrawBorder(this SpriteBatch spriteBatch, DxRectangle rectangle, int borderThickness,
            Color borderColor)
        {
            DrawBorder(spriteBatch, rectangle.ToRectangle(), borderThickness, borderColor);
        }

        /**
            <summary>
                Utilizing an already-initialized SpriteBatch, this will draw a border of the specified thickness around
                the provided rectangle of the specified color.
            </summary>
        */

        public static void DrawBorder(this SpriteBatch spriteBatch, Rectangle rectangle, int borderThickness,
            Color borderColor)
        {
            var coloredPixel = TextureFactory.TextureForColor(borderColor);
            IEnumerable<Rectangle> borderRectangles = GenerateBorderRectangles(rectangle, borderThickness);
            foreach (Rectangle borderRectangle in borderRectangles)
            {
                spriteBatch.Draw(coloredPixel, destinationRectangle: borderRectangle);
            }
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Rectangle destination, Color color)
        {
            var filledCircle = TextureFactory.FilledCircleForColor(color);
            spriteBatch.Draw(filledCircle, destinationRectangle: destination);
        }

        /**
            <summary> 
                Creates 4 rectangles with the specified thickness around the provided rectangle. 
                This has the effect of thick-ifying the provided rectangle's edges. 
            </summary>
        */

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