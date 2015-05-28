using System.Collections.Generic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Utils
{
    public static class SpriteBatchUtils
    {
        private static readonly log4net.ILog LOG =
            log4net.LogManager.GetLogger(typeof (SpriteBatchUtils));

        public static void DrawBorder(DxGame game, DxRectangle rectangle, int borderThickness, Color borderColor)
        {
            DrawBorder(game, rectangle.ToRectangle(), borderThickness, borderColor);
        }

        public static void DrawBorder(DxGame game, Rectangle rectangle, int borderThickness, Color borderColor)
        {
            Texture2D pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData(new[] {borderColor});
            var spriteBatch = game.SpriteBatch;
            IEnumerable<Rectangle> borderRectangles = GenerateBorderRectangles(rectangle, borderThickness);
            foreach (Rectangle borderRectangle in borderRectangles)
            {
                spriteBatch.Draw(pixel, destinationRectangle: borderRectangle);
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
                    case 0:
                        copy.Width = borderThickness;
                        break;
                    case 1:
                        copy.Height = borderThickness;
                        break;
                    case 2:
                        copy.X = copy.X + copy.Width - borderThickness;
                        copy.Width = borderThickness;
                        break;
                    case 3:
                        copy.Y = copy.Y + copy.Width - borderThickness;
                        copy.Height = borderThickness;
                        break;
                }
                generatedBorderRectangles[i] = copy;
            }
            return generatedBorderRectangles;
        }
    }
}