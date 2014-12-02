using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
{
    public class VectorUtils
    {
        private static readonly log4net.ILog LOG =
            log4net.LogManager.GetLogger(typeof(VectorUtils));

        public static Vector2 ConstrainVector(Vector2 vector, float constraintMin, float constraintMax)
        {
            vector.X = MathUtils.Constrain(vector.X, constraintMin, constraintMax);
            vector.Y = MathUtils.Constrain(vector.Y, constraintMin, constraintMax);
            return vector;
        }
    }
}
