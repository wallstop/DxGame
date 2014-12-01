using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public class PhysicsComponent : UpdateableComponent
    {
        virtual public Vector2 Position { get; set; }
        virtual public Vector2 Velocity { get; set; }
        virtual public Vector2 Acceleration { get; set; }

        protected float maxVelocity_;

        public PhysicsComponent(float maxVelocity = 10.0f)
        {
            maxVelocity_ = maxVelocity;
        }

        public PhysicsComponent WithPosition(Vector2 position)
        {
            Position = position;
            return this;
        }

        public PhysicsComponent WithVelocity(Vector2 velocity)
        {
            Velocity = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(Vector2 acceleration)
        {
            Acceleration = acceleration;
            return this;
        }

        new virtual public bool Update()
        {
            Velocity += Acceleration;
            VectorUtils.ConstrainVector(Velocity, maxVelocity_, -maxVelocity_);
            Position += Velocity;
            return base.Update();
        }
    }
}
