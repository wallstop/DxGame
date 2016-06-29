using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Physics
{
    /**
        <summary>
            Basic component that is responsible for managing "Physics" type stuff. Anything that wants to respond to physical interactions
            with other objects needs to have a Physics Component (bullets, map collisions, etc)
        </summary>
    */

    [DataContract]
    [Serializable]
    public class PhysicsComponent : Component
    {
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

        protected PhysicsComponent(DxVector2 velocity, DxVector2 acceleration, SpatialComponent position,
            UpdatePriority updatePriority)
        {
            velocity_ = velocity;
            space_ = position;
            UpdatePriority = updatePriority;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CollisionMessage>(HandleCollisionMessage);
            RegisterMessageHandler<AttachForce>(HandleAttachForce);
            base.OnAttach();
        }

        protected virtual void AttachForce(Force force)
        {
            Validate.Hard.IsNotNull(force, () => this.GetFormattedNullOrDefaultMessage(force));
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
            double scaleAmount = gameTime.ScaleFactor;
            DxVector2 acceleration = new DxVector2();
            foreach(var force in forces)
            {
                force.Update(velocity, acceleration, gameTime);
                if(!force.Dissipated)
                {
                    acceleration += force.Acceleration;
                }
            }
            velocity += acceleration * scaleAmount;
            position += velocity * scaleAmount;
            return Tuple.Create(position, velocity);
        }

        protected override void Update(DxGameTime gameTime)
        {
            var physicsOutput = ForceComputation(gameTime, Position, Velocity, forces_);
            Velocity = physicsOutput.Item2;
            Position = physicsOutput.Item1;
            forces_.RemoveAll(force => force.Dissipated);
        }

        protected void HandleAttachForce(AttachForce forceAttachment)
        {
            AttachForce(forceAttachment.Force);
        }

        protected virtual void HandleCollisionMessage(CollisionMessage message)
        {
            var collisionDirections = message.CollisionDirections;
            var velocity = Velocity;
            // Collide on against y axis (vertical)? Cease movement and acceleration in that direction
            if(collisionDirections.ContainsKey(Direction.East) || collisionDirections.ContainsKey(Direction.West))
            {
                velocity.X = 0;
            }
            // Same for horizontal movement
            if(collisionDirections.ContainsKey(Direction.South) || collisionDirections.ContainsKey(Direction.North))
            {
                velocity.Y = 0;
            }
            Velocity = velocity;
        }

        public class PhysicsComponentBuilder : IBuilder<PhysicsComponent>
        {
            protected readonly HashSet<Force> forces_ = new HashSet<Force>();
            protected DxVector2 acceleration_;
            protected SpatialComponent space_;
            protected UpdatePriority updatePriority_ = UpdatePriority.PHYSICS;
            protected DxVector2 velocity_;

            public virtual PhysicsComponent Build()
            {
                var physics = new PhysicsComponent(velocity_, acceleration_, space_, updatePriority_);
                foreach(var force in forces_)
                {
                    physics.AttachForce(force);
                }
                return physics;
            }

            public virtual PhysicsComponentBuilder WithForce(Force force)
            {
                Validate.Hard.IsNotNull(force, this.GetFormattedNullOrDefaultMessage(force));
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

            public virtual PhysicsComponentBuilder WithGravity()
            {
                forces_.Add(WorldForces.Gravity);
                return this;
            }
        }
    }
}