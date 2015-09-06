﻿using System;

namespace DXGameTest.Core
{
    public static class TestUtils
    {
        public static double NextDouble(this Random rGen, double min, double max)
        {
            return rGen.NextDouble() * (max - min) + min;
        }
    }
}