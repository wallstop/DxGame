using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Models
{
    public static class WorldGravity
    {
        private const float GRAVITY = 0.98f;

        public static void ApplyGravityToPhysics(PhysicsComponent physics)
        {
            DxVector2 acceleration = physics.Acceleration;
            acceleration.Y += GRAVITY;
            physics.Acceleration = acceleration;
        }
    }
}