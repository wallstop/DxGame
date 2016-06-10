using System;
using DxCore.Core.Primitives;

namespace DxCore.Core.Physics
{
    /**
        <summary> 
            A collection of worldly forces that will interact with physical entities. 
            Gravity, drag, etc
        </summary>
    */

    public static class WorldForces
    {
        public static readonly DxVector2 GRAVITY_ACCELERATION = new DxVector2(0, 0.63f);
        private static readonly float DRAG_COEFFICIENT = 0.05f;
        private static readonly float SLOWDOWN_COEFFICIENT = 0.1f;

        private static readonly Tuple<bool, DxVector2> GRAVITY_DISSIPATION_RESULT = Tuple.Create(false,
            GRAVITY_ACCELERATION);

        /* Applies a constant downwards force (down is -y, in terms of graphic space) to an object */

        public static readonly Force Gravity = new Force(new DxVector2(), GRAVITY_ACCELERATION, GravityDissipation,
            "Gravity");

        /* 
            Applies a constant "deceleration in all directions" force to an object, 
            which will gradually slow them down if they are not actively generating velocity 
        */

        public static readonly Force AirResistance = new Force(DxVector2.EmptyVector, DxVector2.EmptyVector, AirResistanceDissipation, "AirResistance");

        /* 
            Applies a more aggressive version of AirResistance, but only in the X direction. 
            This is meant to be used as a conscious force that entities exert upon themselves in an attempt to stop.
        */

        /*
        public static readonly Force Deceleration = new Force(new DxVector2(), new DxVector2(),
            HorizontalVelocityDissipation, "Deceleration");
        */

        public static Tuple<bool, DxVector2> GravityDissipation(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            return GRAVITY_DISSIPATION_RESULT;
        }

        /**
            https://en.wikipedia.org/wiki/Drag_%28physics%29
            <summary> 
                We provide a function that considers an incredibly simplistic model for drag. 
                Instead of basing things off of the object's mass, which would require knowledge of a spatial component, 
                we do some horrible other stuff instead (simply modify the velocity)
            </summary>
        */

        public static Tuple<bool, DxVector2> AirResistanceDissipation(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            var modifiedVelocity = externalVelocity;
            /* Point our acceleration in the opposite direction */
            modifiedVelocity.X = -modifiedVelocity.X;
            modifiedVelocity.Y = -modifiedVelocity.Y;
            /* And scale it way down */
            modifiedVelocity *= DRAG_COEFFICIENT;
            return Tuple.Create(false, modifiedVelocity);
        }

        /**
            <summary>
                A force that applies a stopping force towards an object, slowing them down gradually, where gradually is loosely defined as "by 1/3 of the velocity each frame".

                This force is only applied in the x direction (horizontally)
            </summary>
        */

        [Obsolete("This is not a good way to decelerate things, please come up with something better")]
        public static Tuple<bool, DxVector2> HorizontalVelocityDissipation(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            return Tuple.Create(Math.Abs(externalVelocity.X) > 0,
                new DxVector2 {X = -externalVelocity.X * SLOWDOWN_COEFFICIENT});
        }
    }
}