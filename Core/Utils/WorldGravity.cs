using System;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Utils
{
    [Serializable]
    public class GravityApplier
    {
        private const float Gravity = 0.7f;
        private static readonly DxVector2 GRAVITY_VECTOR = new DxVector2 { X = 0.0f, Y = Gravity };

        private readonly PhysicsComponent physics_;

        public GravityApplier(PhysicsComponent physics)
        {
            physics_ = physics;
            physics_.AddPostUpdater(ApplyGravityToPhysics);
        }

        public void ApplyGravityToPhysics(DxGameTime gameTime)
        {
            var scaleFactor = gameTime.DetermineScaleFactor(DxGame.Instance);
            physics_.Acceleration += (GRAVITY_VECTOR * scaleFactor);
        }

        public static void ApplyGravityToPhysics(PhysicsComponent physics)
        {
            new GravityApplier(physics);
        }
    }
}