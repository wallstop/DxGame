﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public class PhysicsComponent : Component
    {
        [DataMember] protected DxVector2 acceleration_;
        [DataMember] protected DxVector2 maxAcceleration_;
        [DataMember] protected DxVector2 maxVelocity_;
        [DataMember] protected PositionalComponent position_;
        [DataMember] protected DxVector2 velocity_;

        public virtual DxVector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = VectorUtils.ClampVector(value, maxVelocity_); }
        }

        public virtual DxVector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = VectorUtils.ClampVector(value, maxAcceleration_); }
        }

        public virtual DxVector2 Position
        {
            get { return position_.Position; }
            set { position_.Position = value; }
        }

        public virtual PositionalComponent PositionalComponent
        {
            get { return position_; }
            set { WithPositionalComponent(value); }
        }

        public PhysicsComponent(DxGame game)
            : base(game)
        {
            RegisterMessageHandler(typeof (CollisionMessage), HandleCollisionMessage);
            // TODO: Un-hardcode these
            maxVelocity_ = new DxVector2(7.5f, 7.5f);
            maxAcceleration_ = new DxVector2(7.5f, 7.5f);
            UpdatePriority = UpdatePriority.PHYSICS;
        }

        public PhysicsComponent WithVelocity(DxVector2 velocity)
        {
            Validate.IsNotNullOrDefault(velocity,
                $"Cannot initialize {GetType()} with a null/default {nameof(velocity)}");
            velocity_ = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(DxVector2 acceleration)
        {
            Validate.IsNotNullOrDefault(acceleration,
                $"Cannot initialize {GetType()} with a null/default {nameof(acceleration)}");
            acceleration_ = acceleration;
            return this;
        }

        public PhysicsComponent WithPositionalComponent(PositionalComponent position)
        {
            Validate.IsNotNullOrDefault(position,
                $"Cannot initialize {GetType()} with a null/default {nameof(position)}");
            position_ = position;
            return this;
        }

        public PhysicsComponent WithMaxVelocity(DxVector2 maxVelocity)
        {
            Validate.IsNotNullOrDefault(maxVelocity,
                $"Cannot initialize {GetType()} with a null/default {nameof(maxVelocity)}");
            maxVelocity_ = maxVelocity;
            return this;
        }

        public PhysicsComponent WithMaxAcceleration(DxVector2 maxAcceleration)
        {
            Validate.IsNotNullOrDefault(maxAcceleration,
                $"Cannot initialize {GetType()} with a null/default maximum {nameof(maxAcceleration)}");
            maxAcceleration_ = maxAcceleration;
            return this;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var scaleAmount = gameTime.DetermineScaleFactor(DxGame);
            DxVector2 velocity = VectorUtils.ClampVector(Velocity + (acceleration_ * scaleAmount), maxVelocity_);
            Velocity = velocity;
            /* 
                We need to update our Position before we update our velocity. Updating position may cause things like map-bumping/clamping, which will update our velocity from under us.
                So, simply updating velocity first, instead of after, will avoid double-writes.
            */
            Position += velocity * scaleAmount;
        }

        protected void HandleCollisionMessage(Message message)
        {
            var messageAsCollision = GenericUtils.CheckedCast<CollisionMessage>(message);
            var collisionDirections = messageAsCollision.CollisionDirections;
            var velocity = Velocity;
            // Check for x-wise collisions 
            var acceleration = Acceleration;
            // Collide on x axis? Cease movement & acceleration in that direction
            if (collisionDirections.Contains(CollisionDirection.East) ||
                collisionDirections.Contains(CollisionDirection.West))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }
            // Same for horizontal movement
            if (collisionDirections.Contains(CollisionDirection.South) ||
                collisionDirections.Contains(CollisionDirection.North))
            {
                velocity.Y = 0;
                acceleration.Y = 0;
            }
            Velocity = velocity;
            Acceleration = acceleration;
        }
    }
}