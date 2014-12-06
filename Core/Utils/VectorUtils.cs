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

        /**
        <summary>
            Given a vector and another vector with values specifying maximum X and Y values, constrains the original vector to be:
                X within [-constraintVector.X, constraintVector.X]
                Y within [-constraintVector.Y, constraintVector.Y]

            <code>
                Vector2 constrained = VectorUtils.ConstrainVector(new Vector2(-200.0f, 200.0f), new Vector2(150, 300)); // constrained == {-150, 200}
            </code>
            <code>
                Vector2 constrained = VectorUtils.ConstrainVector(new Vector2(-300, 350), new Vector(140, 141)); // constrained == {-140, 141}
            </code>
        </summary>
        */
        public static Vector2 ConstrainVector(Vector2 vector, Vector2 constraintVector)
        {
            vector.X = MathUtils.Constrain(vector.X, -constraintVector.X, constraintVector.X);
            vector.Y = MathUtils.Constrain(vector.Y, -constraintVector.Y, constraintVector.Y);
            return vector;
        }
    }
}
