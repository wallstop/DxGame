using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using NLog;
using Component = DxCore.Core.Components.Basic.Component;

namespace DxCore.Core.Components.Advanced.Physics
{
    public delegate void PhysicsInitialization(Body body, Fixture fixture, PhysicsComponent self);

    [Serializable]
    [DataContract]
    public sealed class PhysicsComponent : Component, ISpatial
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private DxVector2 origin_;

        [DataMember] private readonly DxVector2 bounds_;

        [DataMember] private readonly bool gravity_;

        [DataMember] private readonly float friction_;
        [DataMember] private readonly float restitution_;
        [DataMember] private readonly bool directPositionAccess_;

        [DataMember] private PhysicsInitialization initialization_;

        [DataMember] private List<SensorComponent> attachedSensors_;

        [DataMember]
        public float Density { get; private set; }

        /*
            TODO: Decouple body from PhysicsComponent. Our view of PhysicsComponent is really a "Fixture" instead of a "Body"
        */

        [IgnoreDataMember]
        private Body Body { get; set; }

        [DataMember]
        public CollisionGroup CollisionGroup { get; private set; }

        [DataMember]
        public CollisionGroup CollidesWith { get; private set; }

        [IgnoreDataMember]
        public Fixture Fixture { get; private set; }

        [IgnoreDataMember]
        private Fixture SensorFixture { get; set; }

        [DataMember]
        public PhysicsType PhysicsType { get; private set; }

        [IgnoreDataMember]
        public float Height => bounds_.Y;

        [IgnoreDataMember]
        public float Width => bounds_.X;

        [IgnoreDataMember]
        public IEnumerable<SensorComponent> AttachedSensors => attachedSensors_.ToList();

        [IgnoreDataMember]
        public DxVector2 Position
        {
            get { return Body?.Position * WorldModel.FarseerToDxScale ?? DxVector2.EmptyVector; }
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
                        Body.Position = value.Vector2 * WorldModel.DxToFarseerScale;
                    }
                }
                else
                {
                    Logger.Debug("Ignoring direct position set of {0}", value);
                }
            }
        }

        [IgnoreDataMember]
        public DxVector2 WorldCoordinates => Position;

        [IgnoreDataMember]
        public DxRectangle Space => new DxRectangle(Position.X, Position.Y, Width, Height);

        [IgnoreDataMember]
        public DxVector2 Center => Space.Center;

        private PhysicsComponent(DxVector2 origin, DxVector2 bounds, CollisionGroup collidesWith,
            CollisionGroup collisionGroup, PhysicsType physicsType, float density, bool gravityOn, float restitution,
            float friction, bool directPositionAccess, PhysicsInitialization initialization)
        {
            attachedSensors_ = new List<SensorComponent>();

            origin_ = origin;
            bounds_ = bounds;
            gravity_ = gravityOn;
            restitution_ = restitution;
            friction_ = friction;
            directPositionAccess_ = directPositionAccess;
            initialization_ = initialization;
            PhysicsType = physicsType;
            CollidesWith = collidesWith;
            CollisionGroup = collisionGroup;
            Density = density;
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

        public void Attach(SensorComponent sensor)
        {
            if(Validate.Check.IsNullOrDefault(sensor))
            {
                Logger.Debug("Ignoring attachment of null {0}", typeof(SensorComponent));
                return;
            }

            if(Validate.Check.IsElementOf(attachedSensors_, sensor))
            {
                Logger.Debug("Ignoring double sensor registration of {0}", sensor);
                return;
            }

            attachedSensors_.Add(sensor);
            AttachSensor(sensor);
        }

        private void AttachSensor(SensorComponent sensor)
        {
            if(ReferenceEquals(Fixture, null))
            {
                return;
            }
            foreach(OnCollisionEventHandler onCollisionHandler in sensor.OnCollisionEventHandlers)
            {
                SensorFixture.OnCollision += onCollisionHandler;
            }
            foreach(BeforeCollisionEventHandler beforeCollisionHandler in sensor.BeforeCollisionEventHandlers)
            {
                SensorFixture.BeforeCollision += beforeCollisionHandler;
            }
            foreach(AfterCollisionEventHandler afterCollisionHandler in sensor.AfterCollisionEventHandlers)
            {
                SensorFixture.AfterCollision += afterCollisionHandler;
            }
        }

        public void Detach(SensorComponent sensor)
        {
            if(Validate.Check.IsNullOrDefault(sensor))
            {
                Logger.Debug("Ignoring detachment of null {0}", typeof(SensorComponent));
                return;
            }

            bool removeSuccessful = attachedSensors_.Remove(sensor);

            if(!removeSuccessful)
            {
                Logger.Debug("Ignoring detachment of {0} (not attached)", sensor);
                return;
            }

            foreach(OnCollisionEventHandler onCollisionHandler in sensor.OnCollisionEventHandlers)
            {
                SensorFixture.OnCollision -= onCollisionHandler;
            }
            foreach(BeforeCollisionEventHandler beforeCollisionHandler in sensor.BeforeCollisionEventHandlers)
            {
                SensorFixture.BeforeCollision -= beforeCollisionHandler;
            }
            foreach(AfterCollisionEventHandler afterCollisionHandler in sensor.AfterCollisionEventHandlers)
            {
                SensorFixture.AfterCollision -= afterCollisionHandler;
            }
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

        private void HandleForceAttachment(Force force)
        {
            Body.ApplyForce(force.Value.Vector2, Body.WorldCenter);
        }

        private void HandleImpulseAttachment(Core.Physics.Impulse impulse)
        {
            switch(Body.BodyType)
            {
                /* Depending on body type, ApplyLinearImpulse may be a simple no-op */
                case BodyType.Dynamic:
                {
                    Body.ApplyLinearImpulse(impulse.Value.Vector2, Body.WorldCenter);
                    break;
                }
                case BodyType.Kinematic:
                {
                    Body.LinearVelocity += impulse.Value.Vector2;
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
            DxVector2 negationVelocity = Body.LinearVelocity;
            switch(nullification.Axis)
            {
                case Axis.X:
                {
                    negationVelocity.X = 0;
                    break;
                }

                case Axis.Y:
                {
                    negationVelocity.Y = 0;
                    break;
                }
                default:
                {
                    throw new InvalidEnumArgumentException(
                        $"Unknown {typeof(Axis)}: {nullification.Axis} used in {typeof(Nullification)}");
                }
            }
            Body.LinearVelocity = negationVelocity.Vector2;
        }

        public override void Initialize()
        {
            World gameWorld = DxGame.Instance.Model<WorldModel>().World;

            PolygonShape bounds =
                new PolygonShape(
                    new DxRectangle(0, 0, Width, Height).Vertices()
                        .Select(vertex => vertex * WorldModel.DxToFarseerScale)
                        .ToVertices(), Density);
            
            Body = new Body(gameWorld, origin_.Vector2 * WorldModel.DxToFarseerScale, 0, this)
            {
                BodyType = ResolveCollisionType(PhysicsType),
                FixedRotation = true
            };

            if(!gravity_)
            {
                Body.IgnoreGravity = true;
            }

            Fixture = Body.CreateFixture(bounds, this);
            Fixture.CollidesWith = CollidesWith.CollisionCategory;
            Fixture.CollisionCategories = CollisionGroup.CollisionCategory;

            // TODO: Remove this
            SensorFixture = Fixture;
            foreach(SensorComponent sensor in attachedSensors_)
            {
                AttachSensor(sensor);
            }

            Body.Restitution = restitution_;
            Body.Friction = friction_;
            base.Initialize();

            if(!ReferenceEquals(initialization_, null))
            {
                initialization_.Invoke(Body, Fixture, this);
            }
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

        public static PhysicsComponentBuilder Builder()
        {
            return new PhysicsComponentBuilder();
        }

        public class PhysicsComponentBuilder : IBuilder<PhysicsComponent>
        {
            private const float Density = 0.0f;

            private float density_ = Density;
            private DxVector2? position_;
            private DxVector2? bounds_;
            private bool gravity_ = true;
            private bool directPositionAccess_;
            private float restitution_;
            private PhysicsType physicsType_ = PhysicsType.Dynamic;
            private float friction_ = 1f;
            private CollisionGroup collisionGroup_ = CollisionGroup.All;
            private CollisionGroup collidesWith_ = CollisionGroup.All;
            private PhysicsInitialization initialization_;

            public PhysicsComponentBuilder WithPhysicsInitialization(PhysicsInitialization postInit)
            {
                initialization_ = postInit;
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

            public PhysicsComponentBuilder WithoutRestitution()
            {
                return WithRestitution(0);
            }

            public PhysicsComponentBuilder WithRestitution(double restitution = 1)
            {
                restitution_ = (float) restitution;
                return this;
            }

            public PhysicsComponentBuilder WithoutFriction()
            {
                return WithFriction(0);
            }

            public PhysicsComponentBuilder WithFriction(double friction = 1)
            {
                friction_ = (float) friction;
                return this;
            }

            public PhysicsComponentBuilder WithDensity(double density)
            {
                density_ = (float) density;
                return this;
            }

            public PhysicsComponentBuilder WithoutGravity()
            {
                return WithGravity(false);
            }

            public PhysicsComponentBuilder WithGravity(bool gravityOn = true)
            {
                gravity_ = gravityOn;
                return this;
            }

            public PhysicsComponentBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public PhysicsComponentBuilder WithBounds(DxVector2 bounds)
            {
                bounds_ = bounds;
                return this;
            }

            public PhysicsComponentBuilder WithCollisionType(PhysicsType physicsType)
            {
                physicsType_ = physicsType;
                return this;
            }

            public PhysicsComponentBuilder WithoutCollision()
            {
                WithCollisionGroup(CollisionGroup.None);
                WithCollidesWith(CollisionGroup.None);
                return this;
            }

            public PhysicsComponentBuilder WithDirectPositionAccess(bool directPositionAcces = true)
            {
                directPositionAccess_ = directPositionAcces;
                return this;
            }

            public PhysicsComponentBuilder WithoutDirectPositionAccess()
            {
                return WithDirectPositionAccess(false);
            }

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
                    gravity_, restitution_, friction_, directPositionAccess_, initialization_);
            }
        }
    }
}
