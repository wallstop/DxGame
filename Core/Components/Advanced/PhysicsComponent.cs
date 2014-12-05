using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>

    </summary>
    */
    public class PhysicsComponent : UpdateableComponent
    {
        protected Vector2 acceleration_;
        protected Vector2 maxVelocity_;
        protected Vector2 maxAcceleration_;
        protected PositionalComponent position_;
        protected Vector2 velocity_;
        protected bool isJumping_;

        public PhysicsComponent(float maxVelocityX = 5.0f, float maxVelocityY = 10.0f, float maxAccelerationX = 1.0f,
            float maxAccelerationY = 2.5f, PositionalComponent position = null, GameObject parent = null)
            : base(parent)
        {
            maxVelocity_ = new Vector2(maxVelocityX, maxVelocityY);
            maxAcceleration_ = new Vector2(maxAccelerationX, maxAccelerationY);
            position_ = position;
            isJumping_ = false;
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
            set
            {
                acceleration_.X = MathUtils.Constrain(value.X, -maxAcceleration_.X, maxAcceleration_.X);
                acceleration_.Y = MathUtils.Constrain(value.Y, -maxAcceleration_.Y, maxAcceleration_.Y);
            }
        }

        public PhysicsComponent WithVelocity(Vector2 velocity)
        {
            Debug.Assert(velocity != null, "PhysicsComponent cannot be initialized with null velocity");
            velocity_ = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(Vector2 acceleration)
        {
            Debug.Assert(acceleration != null, "PhysicsComponent cannot be initialized with null acceleration");
            acceleration_ = acceleration;
            return this;
        }

        public PhysicsComponent WithPositionalComponent(PositionalComponent position)
        {
            Debug.Assert(position != null, "PhysicsComponent cannot be initialized with null position");
            position_ = position;
            return this;
        }

        public PhysicsComponent WithMaxVelocity(float maxVelocityX, float maxVelocityY)
        {
            maxVelocity_.X = maxVelocityX;
            maxVelocity_.Y = maxVelocityY;
            return this;
        }

        public PhysicsComponent WithMaxAcceleration(float maxAccelerationX, float maxAccelerationY)
        {
            maxAcceleration_.X = maxAccelerationX;
            maxAcceleration_.Y = maxAccelerationY;
            return this;
        }

        public bool IsJumping
        {
            get { return isJumping_; }
            set { isJumping_ = value; }
        }

        public override bool Update(GameTime gameTime)
        {
            if (position_.Grounded)
            {
                isJumping_ = false;
            }
            Velocity += acceleration_;
            Velocity = new Vector2( MathUtils.Constrain(velocity_.X, -maxVelocity_.X, maxVelocity_.X),
                                    MathUtils.Constrain(velocity_.Y, -maxVelocity_.Y, maxVelocity_.Y));
            position_.Position += Velocity;
            return true;
        }
    }
}