using System;
using System.Collections.Generic;

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
        private const float floatTolerance_ = 0.00001f;
        private const double doubleTolerance_ = 0.0000001;
        public static float FloatTolerance => floatTolerance_;
        public static double DoubleTolerance => doubleTolerance_;
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
            Tests if two floats are equal to within some tolerance of each other. (Floating point numbers are tricky).

            Floating point numbers, by definition, lose precision. This method helps to alleviate some of that issue.

            See: http://en.wikipedia.org/wiki/Loss_of_significance for details

            <code>
                if(0 == FuzzyCompare(x, y, 0.00001)) 
                {
                    // do stuff, they're equal "enough" here
                }
            </code>
        */

        public static int FuzzyCompare(this float lhs, float rhs, float epsilon = floatTolerance_)
        {
            if (Math.Abs(lhs - rhs) <= epsilon)
            {
                return 0;
            }
            return (lhs > rhs ? 1 : -1);
        }

        /**
        Tests if two doubles are equal to within some tolerance of each other. (Floating point numbers are tricky).

            Floating point numbers, by definition, lose precision. This method helps to alleviate some of that issue.

            See: http://en.wikipedia.org/wiki/Loss_of_significance for details

            <code>
                if(0 == FuzzyCompare(x, y, 0.00001)) 
                {
                    // do stuff, they're equal "enough" here
                }
            </code>
        */

        public static int FuzzyCompare(this double lhs, double rhs, double epsilon = doubleTolerance_)
        {
            if (Math.Max(lhs, rhs) - Math.Min(lhs, rhs) <= epsilon)
            {
                return 0;
            }
            return (lhs > rhs ? 1 : -1);
        }

        public static int WrappedAdd(int value, int increment, int max)
        {
            return (value + increment) % max;
        }

        /* Taken from http://stackoverflow.com/questions/4360348/how-to-find-nearest-even-number-for-given-int-given-11-return-12 */
        public static int NearestEven(this int value)
        {
            return (value + 1) ^ ~1;
        }
    }
}
 