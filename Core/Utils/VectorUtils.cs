using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
{
    public class VectorUtils
    {
        public static Vector2 ConstrainVector(Vector2 vector, float constraintMin, float constraintMax)
        {
            vector.X = Constrain(vector.X, constraintMin, constraintMax);
            vector.Y = Constrain(vector.Y, constraintMin, constraintMax);
            return vector;
        }

        private static float Constrain(float value, float constraintMin, float constraintMax)
        {
            value = Math.Min(value, constraintMax);
            value = Math.Max(value, constraintMin);
            return value;
        }

    }
}
