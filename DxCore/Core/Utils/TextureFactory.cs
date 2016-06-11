using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Utils
{
    /**
        TextureFactory allows us to lazily-create color-based Textures, additionally creating only a single texture that is
        shared among all users. Without something like this, components would be wastefully creating Textures on-demand.
        SpriteBatchUtils.DrawBorder is especially guilty of this.

        <summary> Simple factory that serves as a lookup table for textures that are simply plain colors. </summary>
    */

    public class TextureFactory
    {
        private static readonly Lazy<TextureFactory> SINGLETON = new Lazy<TextureFactory>(() => new TextureFactory());
        private readonly Dictionary<Color, Texture2D> texturesForColors_;
        private readonly Dictionary<Color, Texture2D> filledCircleTexturesForColors_; 
        public static TextureFactory Instance => SINGLETON.Value;

        private TextureFactory()
        {
            texturesForColors_ = new Dictionary<Color, Texture2D>();
            filledCircleTexturesForColors_ = new Dictionary<Color, Texture2D>();
        }

        public static Texture2D FilledCircleForColor(Color color)
        {
            if (Instance.filledCircleTexturesForColors_.ContainsKey(color))
            {
                return Instance.filledCircleTexturesForColors_[color];
            }

            var radius = 100;
            var numPixelsPerSide = radius * 2 + 2;
            var filledCircleTexture = new Texture2D(DxGame.Instance.GraphicsDevice, numPixelsPerSide, numPixelsPerSide);

            Color[] data = new Color[numPixelsPerSide * numPixelsPerSide];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.Transparent;
            }

            var angleStep = 1.0 / radius;
            for (var angle = 0.0; angle < Math.PI * 2; angle += angleStep)
            {
                /* Mostly borrowed from http://stackoverflow.com/questions/5641579/xna-draw-a-filled-circle */
                var x = (int) Math.Round(radius + radius * Math.Cos(angle));
                var y = (int) Math.Round(radius + radius * Math.Sin(angle));
                data[x + numPixelsPerSide * y + 1] = color;
            }

            for (int i = 0; i < numPixelsPerSide; ++i)
            {
                int yStart = -1;
                int yEnd = -1;


                //loop through height to find start and end to fill
                for (int j = 0; j < numPixelsPerSide; ++j)
                {

                    if (yStart == -1)
                    {
                        if (j == numPixelsPerSide - 1)
                        {
                            //last row so there is no row below to compare to
                            break;
                        }

                        //start is indicated by Color followed by Transparent
                        if (data[i + (j * numPixelsPerSide)] == color && data[i + ((j + 1) * numPixelsPerSide)] == Color.Transparent)
                        {
                            yStart = j + 1;
                            continue;
                        }
                    }
                    else if (data[i + (j * numPixelsPerSide)] == color)
                    {
                        yEnd = j;
                        break;
                    }
                }

                //if we found a valid start and end position
                if (yStart != -1 && yEnd != -1)
                {
                    //height
                    for (int j = yStart; j < yEnd; j++)
                    {
                        data[i + (j * numPixelsPerSide)] = color;
                    }
                }
            }

            filledCircleTexture.SetData(data);
            Instance.filledCircleTexturesForColors_[color] = filledCircleTexture;
            return filledCircleTexture;
        }

        /**
            <summary> Returns an existing Texture for the color, or creates & caches on on-the-fly </summary>
        */

        public static Texture2D TextureForColor(Color color)
        {
            if (Instance.texturesForColors_.ContainsKey(color))
            {
                return Instance.texturesForColors_[color];
            }

            var coloredTexture = new Texture2D(DxGame.Instance.GraphicsDevice, 1, 1);
            coloredTexture.SetData(new[] {color});
            Instance.texturesForColors_[color] = coloredTexture;
            return coloredTexture;
        }
    }
}