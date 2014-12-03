using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class PhysicsComponent : UpdateableComponent
    {
        protected Vector2 acceleration_;
        protected float maxVelocity_;
        protected float maxAcceleration_;
        protected SpatialComponent space_;
        protected Vector2 velocity_;

        public PhysicsComponent(float maxVelocity = 10.0f, float maxAcceleration = 1.0f,
            SpatialComponent space = null, GameObject parent = null)
            : base(parent)
        {
            maxVelocity_ = maxVelocity;
            maxAcceleration_ = maxAcceleration;
            space_ = space;
            priority_ = UpdatePriority.NORMAL;
        }

        public virtual Vector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = value; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = VectorUtils.ConstrainVector(value, -maxAcceleration_, maxAcceleration_); }
        }

        public PhysicsComponent WithVelocity(Vector2 velocity)
        {
            Debug.Assert(velocity != null, "PhysicsComponent's velocity cannot be initialized to null");
            velocity_ = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(Vector2 acceleration)
        {
            Debug.Assert(acceleration != null, "PhysicsComponent's acceleration cannot be initialized to null");
            acceleration_ = acceleration;
            return this;
        }

        public PhysicsComponent WithSpatialComponent(SpatialComponent space)
        {
            Debug.Assert(space != null, "PhysicsComponent's spatial component cannot be initialized to null");
            space_ = space;
            return this;
        }

        public PhysicsComponent WithMaxVelocity(float maxVelocity)
        {
            maxVelocity_ = maxVelocity;
            return this;
        }

        public PhysicsComponent WithMaxAcceleration(float maxAcceleration)
        {
            maxAcceleration_ = maxAcceleration;
            return this;
        }

        // TODO: Create some kind of prioritization scheme. We want to process input before we process any physics
        public override bool Update(GameTime gameTime)
        {
            Velocity += acceleration_;
            Velocity = VectorUtils.ConstrainVector(velocity_, -maxVelocity_, maxVelocity_);
            space_.Position += Velocity;
            return true;
        }
    }
}