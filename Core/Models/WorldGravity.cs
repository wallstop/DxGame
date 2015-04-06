using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Models
{
    public static class WorldGravity
    {
        private const float GRAVITY = 0.98f;
        /*
            Assumes the provided PhysicsComponent is non-null
        */

        public static void ApplyGravityToPhysics(PhysicsComponent physics)
        {
            physics.Acceleration += GRAVITY_VECTOR;
        }

        private static readonly DxVector2 GRAVITY_VECTOR = new DxVector2 {X = 0.0f, Y = GRAVITY};
    }
}