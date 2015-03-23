using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>

    </summary>
    */

    [Serializable]
    [DataContract]
    public class PhysicsComponent : Component
    {
        [DataMember] protected DxVector2 acceleration_;
        [DataMember] protected DxVector2 maxVelocity_;
        [DataMember] protected DxVector2 maxAcceleration_;
        [DataMember] protected PositionalComponent position_;
        [DataMember] protected DxVector2 velocity_;

        public PhysicsComponent(DxGame game)
            : base(game)
        {
            RegisterMessageHandler(typeof (CollisionMessage), HandleCollisionMessage);
            // TODO: Un-hardcode these
            maxVelocity_ = new DxVector2(5.0f, 5.0f);
            maxAcceleration_ = new DxVector2(5.0f, 5.0f);
            UpdatePriority = UpdatePriority.PHYSICS;
        }

        public virtual DxVector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = VectorUtils.ConstrainVector(value, maxVelocity_); }
        }

        public virtual DxVector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = VectorUtils.ConstrainVector(value, maxAcceleration_); }
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

        public PhysicsComponent WithVelocity(DxVector2 velocity)
        {
            Debug.Assert(velocity != null, "PhysicsComponent cannot be initialized with null velocity");
            velocity_ = velocity;
            return this;
        }

        public PhysicsComponent WithAcceleration(DxVector2 acceleration)
        {
            Debug.Assert(acceleration != null, "PhysicsComponent cannot be initialized with null acceleration");
            acceleration_ = acceleration;
            return this;
        }

        public PhysicsComponent WithPositionalComponent(PositionalComponent position)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(position),
                "PhysicsComponent cannot be initialized with null position");
            position_ = position;
            return this;
        }

        public PhysicsComponent WithMaxVelocity(DxVector2 maxVelocity)
        {
            Debug.Assert(maxVelocity != null, "PhysicsComponent cannot be initialized with a null maximum velocity ");
            maxVelocity_ = maxVelocity;
            return this;
        }

        public PhysicsComponent WithMaxAcceleration(DxVector2 maxAcceleration)
        {
            Debug.Assert(maxAcceleration != null,
                "PhysicsComponent cannot be initialized with a null maximum acceleration ");
            maxAcceleration_ = maxAcceleration;
            return this;
        }

        public override void Update(DxGameTime gameTime)
        {
            DxVector2 acceleration = Acceleration;
            DxVector2 velocity = VectorUtils.ConstrainVector(Velocity + acceleration_, maxVelocity_);
            position_.Position += velocity;

            Velocity = velocity;
            Acceleration = acceleration;
        }

        protected void HandleCollisionMessage(Message message)
        {
            var messageAsCollision = GenericUtils.CheckedCast<CollisionMessage>(message);
            var collisionDirections = messageAsCollision.CollisionDirections;
            var velocity = Velocity;
            // Check for x-wise collisions 
            var acceleration = Acceleration;
            if (collisionDirections.Contains(CollisionDirection.East) ||
                collisionDirections.Contains(CollisionDirection.West))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }
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