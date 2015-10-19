using System;
using System.Collections.Generic;
using System.Linq;
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
        /* Currently acting forces on this object. This will typically include gravity & air resistance */
        [DataMember] protected readonly List<Force> forces_ = new List<Force>();
        [DataMember] protected SpatialComponent space_;
        [DataMember] protected DxVector2 velocity_;
        public IEnumerable<Force> Forces => forces_;

        public virtual DxVector2 Velocity
        {
            get { return velocity_; }
            set { velocity_ = value; }
        }

        public virtual DxVector2 Position
        {
            get { return space_.Position; }
            set { space_.Position = value; }
        }

        public DxRectangle Space => space_.Space;
        public virtual SpatialComponent SpatialComponent => space_;

        protected PhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration, SpatialComponent position,
            UpdatePriority updatePriority)
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleCollisionMessage);
            velocity_ = velocity;
            space_ = position;
            UpdatePriority = updatePriority;
        }

        public virtual void AttachForce(Force force)
        {
            Validate.IsNotNull(force, StringUtils.GetFormattedNullOrDefaultMessage(this, force));
            forces_.Add(force);
            Velocity += force.InitialVelocity;
        }

        public static PhysicsComponentBuilder Builder()
        {
            return new PhysicsComponentBuilder();
        }
        
        public static Tuple<DxVector2, DxVector2> ForceComputation(DxGameTime gameTime, DxVector2 position,
            DxVector2 velocity, List<Force> forces)
        {
            var scaleAmount = gameTime.DetermineScaleFactor(DxGame.Instance);
            var acceleration = new DxVector2();
            foreach (var force in forces)
            {
                force.Update(velocity, acceleration, gameTime);
                if (!force.Dissipated)
                {
                    acceleration += force.Acceleration;
                }
            }
            velocity += (acceleration * scaleAmount);
            position += (velocity * scaleAmount);
            return Tuple.Create(position, velocity);
        }

        protected override void Update(DxGameTime gameTime)
        {
            var physicsOutput = ForceComputation(gameTime, Position, Velocity, forces_);
            Position = physicsOutput.Item1;
            Velocity = physicsOutput.Item2;
            forces_.RemoveAll(force => force.Dissipated);
        }

        protected void HandleCollisionMessage(CollisionMessage message)
        {
            var collisionDirections = message.CollisionDirections;
            var velocity = Velocity; 
            // Collide on against y axis (vertical)? Cease movement and acceleration in that direction
            if (collisionDirections.Contains(Direction.East) ||
                collisionDirections.Contains(Direction.West))
            {
                velocity.X = 0;
            }
            // Same for horizontal movement
            if (collisionDirections.Contains(Direction.South) ||
                collisionDirections.Contains(Direction.North))
            {
                velocity.Y = 0;
            }
            Velocity = velocity;
        }

        public class PhysicsComponentBuilder : IBuilder<PhysicsComponent>
        {
            protected readonly HashSet<Force> forces_ = new HashSet<Force>();
            protected DxVector2 acceleration_;
            protected DxGame game_;
            protected SpatialComponent space_;
            protected UpdatePriority updatePriority_ = UpdatePriority.PHYSICS;
            protected DxVector2 velocity_;

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
        }
    }
}