﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using NLog;
using WallNetCore.Validate;
using Component = DxCore.Core.Components.Basic.Component;

namespace DxCore.Core.Components.Advanced.Physics
{
    public delegate void PhysicsInitialization(Body body, Fixture fixture, PhysicsComponent self);

    [Serializable]
    [DataContract]
    public sealed class PhysicsComponent : Component, ISpatial
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly DxVector2 bounds_;
        [DataMember] private readonly bool directPositionAccess_;

        [DataMember] private readonly float friction_;

        [DataMember] private readonly bool gravity_;
        [DataMember] private readonly float restitution_;
        [DataMember] private readonly bool worldCollisionSensor_;

        [DataMember] private PhysicsInitialization initialization_;

        [DataMember] private DxVector2 origin_;

        [IgnoreDataMember]
        public DxVector2 Center => Space.Center;

        [DataMember]
        public CollisionGroup CollidesWith { get; private set; }

        [DataMember]
        public CollisionGroup CollisionGroup { get; private set; }

        [DataMember]
        public float Density { get; private set; }

        [IgnoreDataMember]
        public Fixture Fixture { get; private set; }

        [IgnoreDataMember]
        public float Height => bounds_.Y;

        [DataMember]
        public PhysicsType PhysicsType { get; private set; }

        [IgnoreDataMember]
        public DxVector2 Position
        {
            get { return Body?.Position * WorldService.FarseerToDxScale ?? DxVector2.EmptyVector; }
            set
            {
                if(directPositionAccess_)
                {
                    // TODO: Fix the shit out of this
                    if(ReferenceEquals(Body, null))
                    {
                        origin_ = value;
                    }
                    else
                    {
                        Body.Position = value.Vector2 * WorldService.DxToFarseerScale;
                    }
                }
                else
                {
                    Logger.Debug("Ignoring direct position set of {0}", value);
                }
            }
        }

        [IgnoreDataMember]
        public float Width => bounds_.X;

        /*
            TODO: Decouple body from PhysicsComponent. Our view of PhysicsComponent is really a "Fixture" instead of a "Body"
        */

        [IgnoreDataMember]
        private Body Body { get; set; }

        private PhysicsComponent(DxVector2 origin, DxVector2 bounds, CollisionGroup collidesWith,
            CollisionGroup collisionGroup, PhysicsType physicsType, float density, bool gravityOn, float restitution,
            float friction, bool directPositionAccess, bool worldCollisionSensor, PhysicsInitialization initialization)
        {
            origin_ = origin;
            bounds_ = bounds;
            gravity_ = gravityOn;
            restitution_ = restitution;
            friction_ = friction;
            directPositionAccess_ = directPositionAccess;
            worldCollisionSensor_ = worldCollisionSensor;
            initialization_ = initialization;
            PhysicsType = physicsType;
            CollidesWith = collidesWith;
            CollisionGroup = collisionGroup;
            Density = density;
        }

        [IgnoreDataMember]
        public DxVector2 WorldCoordinates => Position;

        [IgnoreDataMember]
        public DxRectangle Space => new DxRectangle(Position.X, Position.Y, Width, Height);

        public static PhysicsComponentBuilder Builder()
        {
            return new PhysicsComponentBuilder();
        }

        public override void Initialize()
        {
            World gameWorld = DxGame.Instance.Service<WorldService>().World;

            PolygonShape bounds =
                new PolygonShape(
                    new DxRectangle(0, 0, Width, Height).Vertices()
                        .Select(vertex => vertex * WorldService.DxToFarseerScale)
                        .ToVertices(), Density);

            Body = new Body(gameWorld, origin_.Vector2 * WorldService.DxToFarseerScale, 0, userdata: this)
            {
                BodyType = ResolveCollisionType(PhysicsType),
                FixedRotation = true
            };

            if(!gravity_)
            {
                Body.IgnoreGravity = true;
            }

            Fixture = Body.CreateFixture(bounds, this);
            Body.CollidesWith = CollisionGroup.Map;
            Body.IgnoreCCDWith = CollisionGroup.MovementSensors.CollisionCategory | CollisionGroup.Entities;
            Body.CollisionCategories = CollisionGroup.Entities;
            Body.Restitution = restitution_;
            Body.Friction = friction_;
            base.Initialize();

            if(worldCollisionSensor_)
            {
                SetupWorldCollisionSensor();
            }

            if(!ReferenceEquals(initialization_, null))
            {
                initialization_.Invoke(Body, Fixture, this);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PhysicsAttachment>(HandlePhysicsAttachment);
            base.OnAttach();
        }

        public override void Remove()
        {
            WorldService worldService = DxGame.Instance.Service<WorldService>();
            worldService.World.RemoveBody(Body);
            Body = null;
            base.Remove();
        }

        private void HandleForceAttachment(Force force)
        {
            Body.ApplyForce(force.Value.FarseerScaled().Vector2, Body.WorldCenter);
        }

        private void HandleImpulseAttachment(Core.Physics.Impulse impulse)
        {
            switch(Body.BodyType)
            {
                /* Depending on body type, ApplyLinearImpulse may be a simple no-op */
                case BodyType.Dynamic:
                {
                    Body.ApplyLinearImpulse(impulse.Value.FarseerScaled().Vector2, Body.WorldCenter);
                    break;
                }
                default:
                {
                    Logger.Debug("Ignoring impulse {0} for {1}", impulse, Id);
                    break;
                }
            }
        }

        private void HandleNullificationAttachment(Nullification nullification)
        {
            DxVector2 velocityToNegate = nullification.Value;

            /* 
                QUICK AND DIRTY BOYS 

                Note: This is likely super buggy in the case of non-commandment driven forces interacting with bodies and 
                the player wanting to "stop".

                O WELL, burn that bridge when we come to it
                
            */
            Vector2 linearVelocity = Body.LinearVelocity;
            if((velocityToNegate.X < 0) && (linearVelocity.X < 0))
            {
                linearVelocity.X = 0;
            }
            else if((velocityToNegate.X > 0) && (linearVelocity.X > 0))
            {
                linearVelocity.X = 0;
            }

            if((velocityToNegate.Y < 0) && (linearVelocity.Y < 0))
            {
                linearVelocity.Y = 0;
            }
            else if((velocityToNegate.Y > 0) && (linearVelocity.Y > 0))
            {
                linearVelocity.Y = 0;
            }

            Body.LinearVelocity = linearVelocity;
        }

        private void HandlePhysicsAttachment(PhysicsAttachment physicsAttachment)
        {
            // TODO: Route cleaner
            object physicsInteraction;
            Type attachmentType = physicsAttachment.Extract(out physicsInteraction);
            if(attachmentType == typeof(Force))
            {
                HandleForceAttachment((Force) physicsInteraction);
                return;
            }
            if(attachmentType == typeof(Core.Physics.Impulse))
            {
                HandleImpulseAttachment((Core.Physics.Impulse) physicsInteraction);
                return;
            }
            if(attachmentType == typeof(Nullification))
            {
                HandleNullificationAttachment((Nullification) physicsInteraction);
                return;
            }
            Logger.Warn("No {0} handler found for {1}", typeof(PhysicsAttachment), physicsAttachment);
        }

        private static BodyType ResolveCollisionType(PhysicsType physicsType)
        {
            switch(physicsType)
            {
                case PhysicsType.Dynamic:
                {
                    return BodyType.Dynamic;
                }
                case PhysicsType.Static:
                {
                    return BodyType.Static;
                }
                case PhysicsType.Kinematic:
                {
                    return BodyType.Kinematic;
                }
                default:
                {
                    throw new InvalidEnumArgumentException(
                        $"Unknown {typeof(BodyType)} for {typeof(PhysicsType)} {physicsType}");
                }
            }
        }

        /* Enables a PhysicsComponent to emit CollisionMessages when it collides with the world */

        private void SetupWorldCollisionSensor()
        {
            // TODO: Expand to all directions

            /* 
                Note: This sensor will emit a message for *EACH* map tile that it collides with, per frame. 
                Might want to fix that
            */
            AABB fixtureBounds;
            Fixture.GetAABB(out fixtureBounds, 0);

            const float scalarSoEdgesArentAlwaysColliding = 0.95f;

            Dictionary<Direction, DxLineSegment> edges = (fixtureBounds.ToDxRectangle() - Body.Position).Edges;
            foreach(KeyValuePair<Direction, DxLineSegment> directionAndEdge in edges)
            {
                Direction collisionDirection = directionAndEdge.Key;
                DxLineSegment shrunkJustALittle = directionAndEdge.Value.ScaleInPlace(scalarSoEdgesArentAlwaysColliding);
                Vector2 start = shrunkJustALittle.Start.Vector2;
                Vector2 end = shrunkJustALittle.End.Vector2;
                Fixture mapCollisionSensor = FixtureFactory.AttachEdge(start, end, Body, null);
                mapCollisionSensor.CollidesWith = CollisionGroup.Map;
                mapCollisionSensor.CollisionCategories = CollisionGroup.MovementSensors;
                mapCollisionSensor.IsSensor = true;
                mapCollisionSensor.IgnoreCCDWith = CollisionGroup.MovementSensors.CollisionCategory ^
                                                   CollisionGroup.Entities ^ CollisionGroup.All.CollisionCategory;

                if((collisionDirection == Direction.South) || (collisionDirection == Direction.North))
                {
                    mapCollisionSensor.OnCollision += (self, maybeMapTile, contact) =>
                    {
                        IWorldCollidable worldCollidable = maybeMapTile.UserData as IWorldCollidable;
                        if(ReferenceEquals(worldCollidable, null))
                        {
                            return false;
                        }
                        CollisionMessage worldCollision = new CollisionMessage();
                        worldCollision.WithDirectionAndSource(collisionDirection, worldCollidable);
                        worldCollision.Target = Parent.Id;
                        worldCollision.Emit();
                        return false;
                    };
                }
                if((collisionDirection == Direction.East) || (collisionDirection == Direction.West))
                {
                    mapCollisionSensor.OnCollision += (self, maybeMapTile, contact) =>
                    {
                        IWorldCollidable worldCollidable = maybeMapTile.UserData as IWorldCollidable;
                        if(ReferenceEquals(worldCollidable, null))
                        {
                            return false;
                        }
                        CollisionMessage worldCollision = new CollisionMessage();
                        worldCollision.WithDirectionAndSource(collisionDirection, worldCollidable);
                        worldCollision.Target = Parent.Id;
                        worldCollision.Emit();
                        return false;
                    };
                }
            }
        }

        public class PhysicsComponentBuilder : IBuilder<PhysicsComponent>
        {
            private const float Density = 0.0f;
            private DxVector2? bounds_;
            private CollisionGroup collidesWith_ = CollisionGroup.All;
            private CollisionGroup collisionGroup_ = CollisionGroup.All;

            private float density_ = Density;
            private bool directPositionAccess_;
            private float friction_ = 1f;
            private bool gravity_ = true;
            private PhysicsInitialization initialization_;
            private PhysicsType physicsType_ = PhysicsType.Dynamic;
            private DxVector2? position_;
            private float restitution_;
            private bool worldCollisionSensor_;

            public PhysicsComponent Build()
            {
                Validate.Hard.IsNotNullOrDefault(position_);
                Validate.Hard.IsNotNullOrDefault(bounds_);
                Validate.Hard.IsNotNegative(density_);
                Validate.Hard.IsNotNullOrDefault(collisionGroup_);
                Validate.Hard.IsNotNullOrDefault(collidesWith_);

                DxVector2 position = position_.Value;
                DxVector2 bounds = bounds_.Value;

                return new PhysicsComponent(position, bounds, collidesWith_, collisionGroup_, physicsType_, density_,
                    gravity_, restitution_, friction_, directPositionAccess_, worldCollisionSensor_, initialization_);
            }

            public PhysicsComponentBuilder WithBounds(DxVector2 bounds)
            {
                bounds_ = bounds;
                return this;
            }

            public PhysicsComponentBuilder WithCollidesWith(CollisionGroup collidesWith)
            {
                collidesWith_ = collidesWith;
                return this;
            }

            public PhysicsComponentBuilder WithCollisionGroup(CollisionGroup collisionGroup)
            {
                collisionGroup_ = collisionGroup;
                return this;
            }

            public PhysicsComponentBuilder WithCollisionType(PhysicsType physicsType)
            {
                physicsType_ = physicsType;
                return this;
            }

            public PhysicsComponentBuilder WithDensity(double density)
            {
                density_ = (float) density;
                return this;
            }

            public PhysicsComponentBuilder WithDirectPositionAccess(bool directPositionAcces = true)
            {
                directPositionAccess_ = directPositionAcces;
                return this;
            }

            public PhysicsComponentBuilder WithFriction(double friction = 1)
            {
                friction_ = (float) friction;
                return this;
            }

            public PhysicsComponentBuilder WithGravity(bool gravityOn = true)
            {
                gravity_ = gravityOn;
                return this;
            }

            public PhysicsComponentBuilder WithoutCollision()
            {
                WithCollisionGroup(CollisionGroup.None);
                WithCollidesWith(CollisionGroup.None);
                return this;
            }

            public PhysicsComponentBuilder WithoutDirectPositionAccess()
            {
                return WithDirectPositionAccess(false);
            }

            public PhysicsComponentBuilder WithoutFriction()
            {
                return WithFriction(0);
            }

            public PhysicsComponentBuilder WithoutGravity()
            {
                return WithGravity(false);
            }

            public PhysicsComponentBuilder WithoutRestitution()
            {
                return WithRestitution(0);
            }

            public PhysicsComponentBuilder WithoutWorldCollisionSensor()
            {
                return WithWorldCollisionSensor(false);
            }

            public PhysicsComponentBuilder WithPhysicsInitialization(PhysicsInitialization postInit)
            {
                initialization_ = postInit;
                return this;
            }

            public PhysicsComponentBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public PhysicsComponentBuilder WithRestitution(double restitution = 1)
            {
                restitution_ = (float) restitution;
                return this;
            }

            public PhysicsComponentBuilder WithWorldCollisionSensor(bool sensorOn = true)
            {
                worldCollisionSensor_ = sensorOn;
                return this;
            }
        }
    }
}