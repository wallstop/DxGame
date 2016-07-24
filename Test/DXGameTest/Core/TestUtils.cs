using System;

namespace DXGameTest.Core
{
    public static class TestUtils
    {
        public static double NextDouble(this Random rGen, double min, double max)
        {
            return rGen.NextDouble() * (max - min) + min;
        }

        public static void RunMultipleTimes(this Action function)
        {
            const int numRunTimes = 10000;
            RunMultipleTimes(function, numRunTimes);
        }

        public static void RunMultipleTimes(this Action function, int numRunTimes)
        {
            for(int i = 0; i < numRunTimes; ++i)
            {
                function();
            }
        }
    }
}