using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Models
{
    public class WorldGravityModel : Component
    {
        private const float gravity_ = 0.7f;
        private static readonly HashSet<PhysicsComponent> physics_ = new HashSet<PhysicsComponent>();

        public static float Gravity
        {
            get { return gravity_; }
        }

        public WorldGravityModel(DxGame game)
            : base(game)
        {
        }

        public bool AttachPhysicsComponent(PhysicsComponent physics)
        {
            Debug.Assert(!(GenericUtils.IsNullOrDefault(physics)), "World Gravity Component cannot be assigned to a null physics component");
            bool alreadyExists = physics_.Contains(physics);
            if (!alreadyExists)
            {
                physics_.Add(physics);
            }

            return alreadyExists;
        }

        public override void Update(GameTime gameTime)
        {
            /*
                For now, I'm going to leave it up to each specific physics component to deal
                with the constant deceleration.
            */
            foreach (PhysicsComponent component in physics_)
            {
                Vector2 acceleration = component.Acceleration;
                acceleration.Y += gravity_;
                component.Acceleration = acceleration;
            }
            base.Update(gameTime);
        }
    }
}