using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public class PhysicsComponent : UpdateableComponent
    {
        protected Vector2 acceleration_;
        protected float maxVelocity_;
        protected PositionalComponent position_;
        protected Vector2 velocity_;

        public PhysicsComponent(float maxVelocity = 10.0f, PositionalComponent position = null, GameObject parent = null)
            : base(parent)
        {
            maxVelocity_ = maxVelocity;
            position_ = position;
        }

        public virtual Vector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = value; }
        }

        public virtual Vector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = value; }
        }

        public PhysicsComponent WithVelocity(Vector2 velocity)
        {
            velocity_ = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(Vector2 acceleration)
        {
            acceleration_ = acceleration;
            return this;
        }

        public PhysicsComponent WithPosition(PositionalComponent position)
        {
            position_ = position;
            return this;
        }

        public PhysicsComponent WithMaxVelocity(float maxVelocity)
        {
            maxVelocity_ = maxVelocity;
            return this;
        }

        // TODO: Create some kind of prioritization scheme. We want to process input before we process any physics
        public override bool Update(GameTime gameTime)
        {
            velocity_ += acceleration_;
            VectorUtils.ConstrainVector(velocity_, maxVelocity_, -maxVelocity_);
            Vector2 position = position_.Position;
            position += velocity_;
            position_.Position = position;
            return true;
        }
    }
}