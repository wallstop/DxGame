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
            vector.X = Constrain(vector.X, constraintMin, constraintMax);
            vector.Y = Constrain(vector.Y, constraintMin, constraintMax);
            return vector;
        }

        private static float Constrain(float value, float min, float max)
        {
            if (min > max)
            {
                // Return the original value in the case where we're called with bad values
                LOG.Warn(String.Format("Asked to constrain {0} with bad min {1} max {2}", value, min, max));
                return value;
            }

            value = Math.Max(value, min);
            value = Math.Min(value, max);
            return value;
        }

    }
}
