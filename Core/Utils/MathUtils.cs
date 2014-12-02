using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DXGame.Core.Utils
{
    public static class MathUtils
    {
        public static T Max<T>(T x, T y)
        {
            return (Compare(x, y) > 0) ? x : y;
        }

        public static T Min<T>(T x, T y)
        {
            return (Compare(x, y) < 0) ? x : y;
        }

        public static int Compare<T>(T x, T y)
        {
            return Comparer<T>.Default.Compare(x, y);
        }

        public static T Constrain<T>(T value, T min, T max)
        {
            Debug.Assert(Compare(min, max) < 0,
                String.Format("Could not constrain {0} with min {1}, max {2}", value, min, max));
            value = Max(value, min);
            value = Min(value, max);
            return value;
        }
    }
}