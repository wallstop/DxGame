using System;

namespace DxCore.Core.Utils
{

    public static class RandomUtils
    {
        public static double NextDouble(this Random rGen, double min, double max)
        {
            return min + rGen.NextDouble() * (max - min);
        }
    }
}