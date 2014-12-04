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

        /**
        <summary>
            Given a vector, a minimum value, and a maximum value, constrains both the x and y component of the vector to be within [min, max]

            <code>
                Vector2 constrained = VectorUtils.ConstrainVector(new Vector2(-200.0f, 200.0f), 0.0f, 100.0f); // constrained == {0.0f, 100.0f}
            </code>
            <code>
                Vector2 constrained = VectorUtils.ConstrainVector(new Vector2(-200.0f, 200.0f), -200.0f, 10000.0f); // constrained == {-200.0f, 200.0f}
            </code>
        </summary>
        */
        public static Vector2 ConstrainVector(Vector2 vector, float constraintMin, float constraintMax)
        {
            vector.X = MathUtils.Constrain(vector.X, constraintMin, constraintMax);
            vector.Y = MathUtils.Constrain(vector.Y, constraintMin, constraintMax);
            return vector;
        }
    }
}
