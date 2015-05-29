using System;
using DXGame.Core.Wrappers;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Utils
{
    public static class VectorUtils
    {
        private static readonly log4net.ILog LOG =
            log4net.LogManager.GetLogger(typeof (VectorUtils));

        /**
        <summary>
            Given a vector, a minimum value, and a maximum value, constrains both the x and y component of the vector to be within [min, max]

            <code>
                Vector2 constrained = VectorUtils.ClampVector(new Vector2(-200.0f, 200.0f), 0.0f, 100.0f); // constrained == {0.0f, 100.0f}
            </code>
            <code>
                Vector2 constrained = VectorUtils.ClampVector(new Vector2(-200.0f, 200.0f), -200.0f, 10000.0f); // constrained == {-200.0f, 200.0f}
            </code>
        </summary>
        */

        public static Vector2 ClampVector(Vector2 vector, float constraintMin, float constraintMax)
        {
            vector.X = MathHelper.Clamp(vector.X, constraintMin, constraintMax);
            vector.Y = MathHelper.Clamp(vector.Y, constraintMin, constraintMax);
            return vector;
        }

        /**
        <summary>
            Given a vector and another vector with values specifying maximum X and Y values, constrains the original vector to be:
                X within [-constraintVector.X, constraintVector.X]
                Y within [-constraintVector.Y, constraintVector.Y]

            <code>
                Vector2 constrained = VectorUtils.ClampVector(new Vector2(-200.0f, 200.0f), new Vector2(150, 300)); // constrained == {-150, 200}
            </code>
            <code>
                Vector2 constrained = VectorUtils.ClampVector(new Vector2(-300, 350), new Vector(140, 141)); // constrained == {-140, 141}
            </code>
        </summary>
        */

        public static Vector2 ClampVector(Vector2 vector, Vector2 constraintVector)
        {
            vector.X = MathHelper.Clamp(vector.X, -constraintVector.X, constraintVector.X);
            vector.Y = MathHelper.Clamp(vector.Y, -constraintVector.Y, constraintVector.Y);
            return vector;
        }

        public static DxVector2 ClampVector(DxVector2 vector, DxVector2 constraintVector)
        {
            vector.X = MathHelper.Clamp(vector.X, -constraintVector.X, constraintVector.X);
            vector.Y = MathHelper.Clamp(vector.Y, -constraintVector.Y, constraintVector.Y);
            return vector;
        }

        /**
        <summary>
            Given a vector representing positional coordinates and another vector representing the dimenions,
            constructs a Rectangle that is a "superset" of the Rectangle being represented, but in integer coordinates.
            For example, a Rectangle constructed from (0.5, 0.5, 3.5, 3.5) would be (0, 0, 4, 4)
        </summary>
        */

        public static DxRectangle RectangleFrom(DxVector2 position, DxVector2 dimensions)
        {
            Validate.IsTrue(dimensions.X >= 0);
            Validate.IsTrue(dimensions.Y >= 0);
            return new DxRectangle(
                /*
                    The position may be anywhere, so if it's -.05, we actually want it to wrap to -1. Similarly, 
                    if the position is 0.5, we want it to wrap to 1.0
                */
                (float) Math.Ceiling(Math.Abs(position.X)) * Math.Sign(position.X),
                (float) Math.Ceiling(Math.Abs(position.Y)) * Math.Sign(position.Y),
                (float) Math.Ceiling(dimensions.X), // Assume dimensions are always positive
                (float) Math.Ceiling(dimensions.Y));
        }
    }
}