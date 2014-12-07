using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DXGame.Core.Utils
{
    /**
    <summary>
        Provides a collection of utility functions for general Math-based functions that 
        occur on a regular basis.
    </summary>
    */

    public static class MathUtils
    {
        private static readonly log4net.ILog LOG = log4net.LogManager.GetLogger(typeof (MathUtils));

        /**
        <summary>
            Given two values, returns the maximum of the two.

            <code>
                int max = MathUtils.Max(23, 100); // max == 100
            </code>
        </summary>
        */

        public static T Max<T>(T x, T y)
        {
            return (Compare(x, y) > 0) ? x : y;
        }

        /**
        <summary>
            Given two values, returns the minimum of the two.

            <code>
                int min = MathUtils.Min(23, 100); // min == 23
            </code>
        </summary>
        */

        public static T Min<T>(T x, T y)
        {
            return (Compare(x, y) < 0) ? x : y;
        }

        /**
        <summary>
            Given two values, returns:
                1 if the first parameter is larger than the second
                0 if the two parameters are equal
                -1 if the first parameter is less than the second
            
            <code>
                int value = MathUtils.Compare(2.4f, -5.0f); // value == 1
            </code
        </summary
        */

        public static int Compare<T>(T x, T y)
        {
            return Comparer<T>.Default.Compare(x, y);
        }

        /**
        <summary>
            Given a value, a minimum, and a maximum, constrains the value to be between the two.

            <code>
                int constrained = MathUtils.Constrain(3, 100, 1000); // constrained == 100
            </code>
            <code>
                float constrained = MathUtils.Constrain(0.0f, -10.0f, 10.0f); // constrained == 0.0f;
            </code>
        </summary>
        */

        public static T Constrain<T>(T value, T min, T max)
        {
            Debug.Assert(Compare(min, max) <= 0,
                String.Format("Could not constrain {0} with min {1}, max {2}", value, min, max));
            value = Max(value, min);
            value = Min(value, max);
            return value;
        }

        /**
        <summary>
            Given a value, returns:
                1 if the value is positive (greater than default)
                1 if the value is 0 (or default)
                -1 if the value is negative (less than default)

            <code>
                int sign = MathUtils.SignOf(-300); // sign = -1
            </code>
            <code>
                int sign = MathUtils.SignOf(245.33f); // sign = 1
            </code>
        </summary>
        */

        public static int SignOf<T>(T value)
        {
            return Compare(value, default(T)) < 0 ? -1 : 1; // Count 0 here as positive
        }
    }
}