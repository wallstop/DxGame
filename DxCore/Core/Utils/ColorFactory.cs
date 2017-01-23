using System;
using DxCore.Core.Utils.Cache.Simple;
using Microsoft.Xna.Framework;
using WallNetCore.Validate;

namespace DxCore.Core.Utils
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
        private readonly UnboundedLoadingSimpleCache<Tuple<float, Color>, Color> coloredTransparencies_;
        private readonly UnboundedLoadingSimpleCache<float, Color> transparencies_;
        public static ColorFactory Instance => SINGLETON.Value;

        private ColorFactory()
        {
            transparencies_ =
                new UnboundedLoadingSimpleCache<float, Color>(scalar => new Color(Color.White.ToVector4() * scalar));
            coloredTransparencies_ =
                new UnboundedLoadingSimpleCache<Tuple<float, Color>, Color>(
                    tuple => new Color(tuple.Item2.ToVector4() * tuple.Item1));
        }

        /**

            <summary>
                Retrieves an already cached transparency color for the provided blend value, or creates a new one & caches it
            </summary>
        */

        public static Color Transparency(float transparencyWeight)
        {
            transparencyWeight = RoundTransparencyWeight(transparencyWeight);
            return Instance.transparencies_.Get(transparencyWeight);
        }

        /**
            <summary>
                Retrieves an already cached color of the specified transparency, or creates a new one, caches it, and returns it.
            </summary>
        */

        public static Color Transparency(float transparencyWeight, Color color)
        {
            transparencyWeight = RoundTransparencyWeight(transparencyWeight);
            return Instance.coloredTransparencies_.Get(Tuple.Create(transparencyWeight, color));
        }

        private static float RoundTransparencyWeight(float transparencyWeight)
        {
            Validate.Hard.IsInClosedInterval(transparencyWeight, 0, 1.0);
            /* Clamp transparencyWeight to nearest 0.01 so we reduce the space of our possible key set (by some) */
            return (float) Math.Round(transparencyWeight, NUM_PRECISION_DIGITS);
        }
    }
}