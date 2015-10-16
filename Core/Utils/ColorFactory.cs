using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
{
    /**
        Provides us a cache of transparency values. Creating new Colors is not cheap. 
        If we ever want to do alpha blending (transparent effects, like for shaders and stuff), we'll be creating a lot of "transparent" colors on-the-fly.
        ColorFactory allows us to cache these values for minimal GC :)

        Note: Access to this class is NOT thread-safe

        <summary>
            Non-thread safe Color cache for Transparency values
        </summary>
    */

    public class ColorFactory
    {
        private static readonly int NUM_PRECISION_DIGITS = 2;
        private static readonly Lazy<ColorFactory> SINGLETON = new Lazy<ColorFactory>(() => new ColorFactory());
        private readonly Dictionary<float, Color> transparencies_;
        public static ColorFactory Instance => SINGLETON.Value;

        private ColorFactory()
        {
            transparencies_ = new Dictionary<float, Color>();
        }

        /**

            <summary>
                Retrieves an already cached transparency color for the provided blend value, or creates a new one & caches it
            </summary>
        */

        public static Color Transparency(float alphaBlend)
        {
            /* Wrap to nearest 0.01 so we don't get infinite keys */
            alphaBlend = (float) Math.Round(alphaBlend, NUM_PRECISION_DIGITS);
            if (Instance.transparencies_.ContainsKey(alphaBlend))
            {
                return Instance.transparencies_[alphaBlend];
            }
            Validate.IsTrue(alphaBlend >= 0.0f && alphaBlend <= 1.0f, $"Cannot create a transparency of {alphaBlend}");
            var color = new Color(Color.White.ToVector4() * alphaBlend);
            Instance.transparencies_[alphaBlend] = color;
            return color;
        }
    }
}