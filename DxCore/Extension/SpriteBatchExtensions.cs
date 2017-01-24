using System;
using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Extension
{
    public static class SpriteBatchExtensions
    {
        public enum BorderRenderMode
        {
            Inside, // Border will be rendered completely within the shape
            Outside, // Border will be rendered completed outside the shape
            Mixed // Border will attempt to be balanced in and outside the shape
        }

        public const BorderRenderMode DefaultBorderRenderMode = BorderRenderMode.Inside;

        /**
            <summary>
                Utilizing an already-initialized SpriteBatch, this will draw a border of the specified thickness around
                the provided rectangle of the specified color.
            </summary>
        */

        public static void DrawBorder(this SpriteBatch spriteBatch, Rectangle rectangle, int borderThickness,
            Color borderColor, BorderRenderMode borderRenderMode = DefaultBorderRenderMode)
        {
            Texture2D coloredPixel = TextureFactory.TextureForColor(borderColor);
            IEnumerable<Rectangle> borderRectangles = GenerateBorderRectangles(rectangle, borderThickness,
                borderRenderMode);
            foreach(Rectangle borderRectangle in borderRectangles)
            {
                spriteBatch.Draw(coloredPixel, destinationRectangle: borderRectangle);
            }
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Rectangle destination, Color color,
            float transparencyWeight = 1.0f)
        {
            Texture2D filledCircle = TextureFactory.FilledCircleForColor(color);
            Color transparency = ColorFactory.Transparency(transparencyWeight);
            spriteBatch.Draw(filledCircle, destinationRectangle: destination, color: transparency);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, DxCircle circle, Color color,
            float transparencyWeight = 1.0f)
        {
            DrawCircle(spriteBatch, circle.Bounds, color, transparencyWeight);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, DxVector2 start, DxVector2 end, Color color,
            float thickness, float transparencyWeight = 1.0f)
        {
            DxVector2 displacement = end - start;
            float angle = (float) Math.Atan2(displacement.Y, displacement.X);

            var texture = TextureFactory.TextureForColor(color);
            spriteBatch.Draw(texture,
                destinationRectangle:
                new Rectangle((int) Math.Round(start.X), (int) Math.Round(start.Y),
                    (int) Math.Round(displacement.Magnitude), (int) Math.Round(thickness)), rotation: angle);
        }

        /**
            <summary> 
                Creates 4 rectangles with the specified thickness around the provided rectangle. 
                This has the effect of thick-ifying the provided rectangle's edges. 
            </summary>
        */

        public static IEnumerable<Rectangle> GenerateBorderRectangles(this Rectangle rectangle, int borderThickness,
            BorderRenderMode borderRenderMode = DefaultBorderRenderMode)
        {
            int leftOffset;
            int rightOffset;

            switch(borderRenderMode)
            {
                default:
                case BorderRenderMode.Inside:
                    leftOffset = 0;
                    rightOffset = 0;
                    break;
                case BorderRenderMode.Outside:
                    leftOffset = borderThickness / 2;
                    rightOffset = borderThickness - leftOffset;
                    break;
                case BorderRenderMode.Mixed:
                    int target = (int) Math.Round(borderThickness / 2.0f);
                    leftOffset = target / 2;
                    rightOffset = target - leftOffset;
                    break;
            }

            rectangle.X -= leftOffset;
            rectangle.Width += rightOffset;
            rectangle.Y -= leftOffset;
            rectangle.Height += rightOffset;

            const int numRectangles = 4;
            Rectangle[] generatedBorderRectangles = new Rectangle[numRectangles];
            for(int i = 0; i < numRectangles; ++i)
            {
                Rectangle copy = rectangle;
                switch(i)
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