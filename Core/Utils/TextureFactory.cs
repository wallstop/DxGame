using System;
using System.Collections.Generic;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Utils
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
        public static TextureFactory Instance => SINGLETON.Value;

        private TextureFactory()
        {
            texturesForColors_ = new Dictionary<Color, Texture2D>();
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