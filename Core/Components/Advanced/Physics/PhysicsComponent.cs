using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
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
        protected static readonly float VELOCITY_FLOOR = 0.5f;

        [DataMember] protected DxVector2 acceleration_;
        [DataMember] protected SpatialComponent space_;
        [DataMember] protected DxVector2 velocity_;

        /* Currently acting forces on this object. This will typically include gravity & air resistance */
        [DataMember] protected readonly List<Force> forces_ = new List<Force>();

        public IEnumerable<Force> Forces => forces_; 

        public virtual DxVector2 Velocity
        {
            get { return velocity_; }
            set
            {
                velocity_ = value;
                //velocity_.X = (Math.Abs(value.X) < VELOCITY_FLOOR && Acceleration.X != 0) ? 0 : value.X;
                //velocity_.Y = (Math.Abs(value.Y) < VELOCITY_FLOOR && Acceleration.Y != 0) ? 0 : value.Y;
            }
        }

        public virtual DxVector2 Acceleration
        {
            get { return acceleration_; }
            set { acceleration_ = value; }
        }

        public virtual DxVector2 Position
        {
            get { return space_.Position; }
            set { space_.Position = value; }
        }

        public DxRectangle Space => space_.Space;

        public virtual SpatialComponent SpatialComponent => space_;

        protected PhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration, SpatialComponent position, UpdatePriority updatePriority)
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleCollisionMessage);
            velocity_ = velocity;
            acceleration_ = acceleration;
            space_ = position;
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
            protected SpatialComponent space_;
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

            public virtual PhysicsComponentBuilder WithSpatialComponent(SpatialComponent space)
            {
                space_ = space;
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
                Validate.IsNotNull(space_, StringUtils.GetFormattedNullOrDefaultMessage(this, space_));
                if (game_ == null)
                {
                    game_ = DxGame.Instance;
                }
            }

            public virtual PhysicsComponent Build()
            {
                CheckParameters();
                var physics = new PhysicsComponent(game_, velocity_, acceleration_, space_, updatePriority_);
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
                if (!force.Dissipated)
                {
                    acceleration += force.Acceleration;
                }
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