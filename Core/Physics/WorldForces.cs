using System;
using DXGame.Core.Primitives;

namespace DXGame.Core.Physics
{
    public static class WorldForces
    {
        public static readonly DxVector2 GRAVITY_ACCELERATION = new DxVector2(0, 0.63f);
        //private static readonly float SLOW_DOWN_PER_FRAME = 5f;
        //private static readonly float NO_MOVEMENT_FLOOR
        private static readonly float DRAG_COEFFICIENT = 0.05f;
        //private static readonly float DRAG_FLOOR = 1f;

        private static readonly Tuple<bool, DxVector2> GRAVITY_DISSIPATION_RESULT = Tuple.Create(false,
            GRAVITY_ACCELERATION);

        public static readonly Force Gravity = new Force(new DxVector2(), GRAVITY_ACCELERATION, GravityDissipation, "Gravity");
        public static readonly Force AirResistance = new Force(new DxVector2(), new DxVector2(), AirResistanceDissipation, "AirResistance");
        //public static readonly Force SlowingDown = new Force(new DxVector2(), new DxVector2(), ,  );

        private static Tuple<bool, DxVector2> GravityDissipation(DxVector2 externalVelocity, DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
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
        private static Tuple<bool, DxVector2> AirResistanceDissipation(DxVector2 externalVelocity,
            DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            var modifiedVelocity = externalVelocity;
            /* Point our acceleration in the opposite direction */
            modifiedVelocity.X = -modifiedVelocity.X;
            modifiedVelocity.Y = -modifiedVelocity.Y;
            /* And scale it way down */
            modifiedVelocity *= DRAG_COEFFICIENT;
            return Tuple.Create(false, modifiedVelocity);
        }

        //private static Tuple<bool, DxVector2> SlowingDownDissipation(DxVector2 externalVelocity,
        //    DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
        //{
        //    var accelerationResult = new DxVector2();
        //    if(Math.Abs(externalVelocity.X) < )
        //    if(externalVelocity

        //}


    }
}