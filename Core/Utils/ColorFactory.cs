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

        public static Color Transparency(float transparencyWeight)
        {
            /* transparencyWeight to nearest 0.01 so we don't get infinite keys */
            transparencyWeight = (float) Math.Round(transparencyWeight, NUM_PRECISION_DIGITS);
            if (Instance.transparencies_.ContainsKey(transparencyWeight))
            {
                return Instance.transparencies_[transparencyWeight];
            }
            Validate.IsTrue(transparencyWeight >= 0.0f && transparencyWeight <= 1.0f, $"Cannot create a transparency of {transparencyWeight}");
            var color = new Color(Color.White.ToVector4() * transparencyWeight);
            Instance.transparencies_[transparencyWeight] = color;
            return color;
        }
    }
}