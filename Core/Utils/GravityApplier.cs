using System;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Utils
{
    [Serializable]
    public class GravityApplier
    {
        private const float Gravity = 0.6f;

        private readonly PhysicsComponent physics_;

        public GravityApplier(PhysicsComponent physics)
        {
            physics_ = physics;
            physics_.AddPostUpdater(ApplyGravityToPhysics);
        }

        public void ApplyGravityToPhysics(DxGameTime gameTime)
        {
            // TODO: Concept of disparate acceleration sources (this way we can simply add everything together)
            var acceleration = physics_.Acceleration;
            acceleration.Y = Math.Max(Gravity, acceleration.Y);
            physics_.Acceleration = acceleration;
        }

        public static void ApplyGravityToPhysics(PhysicsComponent physics)
        {
            new GravityApplier(physics);
        }
    }
}