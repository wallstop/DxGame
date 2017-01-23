using System;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Extension
{
    public static class Texture2DExtensions
    {
        /**
            <summary>
                Crops a texture by copying it and grabbing the relevant chunk
            </summary>
            <note>
                The perf on this is terrible, only use if you don't have access to SpriteBatch or something similar
            </note>

            Borrowed from http://gamedev.stackexchange.com/questions/116623/using-sourcerect-or-cropping-out-a-texture-2d-to-draw-monogame-xna
        */

        public static Texture2D Crop(this Texture2D texture, DxRectangle newBounds)
        {
            int width = (int) Math.Round(newBounds.Width);
            int height = (int) Math.Round(newBounds.Height);
            // TODO
            Texture2D cropped = new Texture2D(texture.GraphicsDevice, width, height);
            Color[] imageData = new Color[texture.Width * texture.Height];
            Color[] croppedImageData = new Color[width * height];

            texture.GetData(imageData);

            int index = 0;
            for(int y = (int) Math.Round(newBounds.Y); y < (int) Math.Round(newBounds.Y) + height; ++y)
            {
                for(int x = (int) Math.Round(newBounds.X); x < (int) Math.Round(newBounds.X) + width; ++x)
                {
                    croppedImageData[index] = imageData[y * texture.Width + x];
                    ++index;
                }
            }

            cropped.SetData(croppedImageData);
            return cropped;
        }
    }
}