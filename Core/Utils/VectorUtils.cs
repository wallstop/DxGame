using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
{
    public class VectorUtils
    {
        public static void ConstrainVector(Vector2 vector, float constraintMin, float constraintMax)
        {
            Constrain(ref vector.X, constraintMin, constraintMax);
            Constrain(ref vector.Y, constraintMin, constraintMax);
        }

        private static void Constrain(ref float value, float constraintMin, float constraintMax)
        {
            value = Math.Min(value, constraintMax);
            value = Math.Max(value, constraintMin);
        }

    }
}
