using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Physics
{
    /**
        <summary>
            Basic component that is responsible for managing "Physics" type stuff. Anything that wants to respond to physical interactions
            with other objects needs to have a Physics Component (bullets, map collisions, etc)
        </summary>
    */
    [Serializable]
    [DataContract]
    public class PhysicsComponent : Component
    {
        [DataMember] protected DxVector2 acceleration_;
        [DataMember] protected PositionalComponent position_;
        [DataMember] protected DxVector2 velocity_;

        /* Currently acting forces on this object. This will typically include gravity & air resistance */
        [DataMember] protected readonly List<Force> forces_ = new List<Force>();

        public virtual DxVector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = value; }
        }

        public virtual DxVector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = value; }
        }

        public virtual DxVector2 Position
        {
            get { return position_.Position; }
            set { position_.Position = value; }
        }

        public virtual PositionalComponent PositionalComponent => position_;

        protected PhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration, PositionalComponent position, UpdatePriority updatePriority)
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleCollisionMessage);
            velocity_ = velocity;
            acceleration_ = acceleration;
            position_ = position;
            UpdatePriority = updatePriority;
        }

        public void AttachForce(Force force)
        {
            Validate.IsNotNull(force, StringUtils.GetFormattedNullOrDefaultMessage(this, force));
            forces_.Add(force);
            Velocity += force.InitialVelocity;
        }

        public static PhysicsComponentBuilder Builder()
        {
            return new PhysicsComponentBuilder();
        }

        public class PhysicsComponentBuilder : IBuilder<PhysicsComponent>
        {
            protected DxVector2 velocity_;
            protected DxVector2 acceleration_;
            protected PositionalComponent position_;
            protected UpdatePriority updatePriority_ = UpdatePriority.PHYSICS;
            protected readonly HashSet<Force> forces_ = new HashSet<Force>();
            protected DxGame game_;

            public virtual PhysicsComponentBuilder WithForce(Force force)
            {
                Validate.IsNotNull(force, StringUtils.GetFormattedNullOrDefaultMessage(this, force));
                forces_.Add(force);
                return this;
            }

            public virtual PhysicsComponentBuilder WithUpdatePriority(UpdatePriority updatePriority)
            {
                updatePriority_ = updatePriority;
                return this;
            }

            public virtual PhysicsComponentBuilder WithPositionalComponent(PositionalComponent position)
            {
                position_ = position;
                return this;
            }

            public virtual PhysicsComponentBuilder WithAcceleration(DxVector2 acceleration)
            {
                acceleration_ = acceleration;
                return this;
            }

            public virtual PhysicsComponentBuilder WithVelocity(DxVector2 velocity)
            {
                velocity_ = velocity;
                return this;
            }

            public virtual PhysicsComponentBuilder WithAirResistance()
            {
                forces_.Add(WorldForces.AirResistance);
                return this;
            }

            public virtual PhysicsComponentBuilder WithWorldForces()
            {
                WithAirResistance();
                WithGravity();
                return this;
            }

            public virtual PhysicsComponentBuilder WithGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public virtual PhysicsComponentBuilder WithGravity()
            {
                forces_.Add(WorldForces.Gravity);
                return this;
            }

            protected void CheckParameters()
            {
                Validate.IsNotNull(position_, StringUtils.GetFormattedNullOrDefaultMessage(this, position_));
                if (game_ == null)
                {
                    game_ = DxGame.Instance;
                }
            }

            public virtual PhysicsComponent Build()
            {
                CheckParameters();
                var physics = new PhysicsComponent(game_, velocity_, acceleration_, position_, updatePriority_);
                foreach (var force in forces_)
                {
                    physics.AttachForce(force);
                }
                return physics;
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            var scaleAmount = gameTime.DetermineScaleFactor(DxGame);
            var acceleration = Acceleration;
            foreach (var force in forces_)
            {
                force.Update(Velocity, Acceleration, gameTime);
                /* 
                    Make sure to modify a temporary - we don't want to cumulatively update these things, 
                    we simply want to aggregate their results on velocity 
                */
                acceleration += force.Acceleration;
            }
            /* Scale our acceleration to the elapsed time for the current frame - we don't want the game running at hyperspeed */
            Velocity += (acceleration * scaleAmount);
            /* If our forces are gone, remove them */
            forces_.RemoveAll(force => force.Dissipated);
            Position += Velocity * scaleAmount;
        }

        protected void HandleCollisionMessage(CollisionMessage message)
        {
            var collisionDirections = message.CollisionDirections;
            var velocity = Velocity;
            // Check for x-wise collisions 
            var acceleration = Acceleration;
            // Collide on against y axis (vertical)? Cease movement & acceleration in that direction
            if (collisionDirections.Contains(Direction.East) ||
                collisionDirections.Contains(Direction.West))
            {
                velocity.X = 0;
                acceleration.X = 0;
            }
            // Same for horizontal movement
            if (collisionDirections.Contains(Direction.South) ||
                collisionDirections.Contains(Direction.North))
            {
                velocity.Y = 0;
                acceleration.Y = 0;
            }
            Velocity = velocity;
            Acceleration = acceleration;
        }
    }
}