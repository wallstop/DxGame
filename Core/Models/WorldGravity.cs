using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public static class WorldGravity
    {
        private const float Gravity = 0.98f;
        private static readonly DxVector2 GRAVITY_VECTOR = new DxVector2 {X = 0.0f, Y = Gravity};
        /*
            Assumes the provided PhysicsComponent is non-null
        */

        public static void ApplyGravityToPhysics(DxGameTime gameTime, DxGame game, PhysicsComponent physics)
        {
            var scaleFactor = gameTime.DetermineScaleFactor(game);
            physics.Acceleration += (GRAVITY_VECTOR * scaleFactor);
        }
    }
}