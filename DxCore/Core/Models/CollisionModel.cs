using System;
using System.Collections.Generic;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Models
{
    public class CollisionModel : Model
    {
        private const float StepRate = 1 / 60.0f;
        private static readonly TimeSpan TargetFps = TimeSpan.FromSeconds(StepRate);

        private TimeSpan LastTicked { get; set; }

        public World World { get; }

        public CollisionModel()
        {
            World = new World(new Vector2(0, 9.82f));
            LastTicked = TimeSpan.Zero;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PhysicsMessage>(HandlePhysicsMessage);
        }

        protected override void Update(DxGameTime gameTime)
        {
            RateLimitedUpdate(gameTime);
        }

        private void RateLimitedUpdate(DxGameTime gameTime)
        {
            if(LastTicked + TargetFps < gameTime.TotalGameTime)
            {
                LastTicked = gameTime.TotalGameTime;
                World.Step(StepRate);
            }
        }

        private void FullThrottleUpdate(DxGameTime gameTime)
        {
            World.Step((float) gameTime.ElapsedGameTime.TotalSeconds);
        }

        private void HandlePhysicsMessage(PhysicsMessage message)
        {
            HashSet<Fixture> affectedFixtures = new HashSet<Fixture>();

            foreach(DxRectangle area in message.AffectedAreas)
            {
                AABB affectedArea = area.Aabb();
                foreach(Fixture fixture in World.QueryAABB(ref affectedArea))
                {
                    affectedFixtures.Add(fixture);
                }
            }
            foreach(Fixture fixture in affectedFixtures)
            {
                message.Interaction(message.Source, (PhysicsComponent) fixture.UserData);
            }
        }
    }
}