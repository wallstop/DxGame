using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DxCore.Core.Messaging.Physics;
using DxCore.Core.Models;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using NLog;
using Component = DxCore.Core.Components.Basic.Component;

namespace DxCore.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public class PhysicsComponent : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly DxVector2 origin_;

        [DataMember] private readonly DxVector2 bounds_;

        [DataMember] private readonly bool gravity_;

        [DataMember] private readonly float friction_;
        [DataMember] private readonly float restitution_;

        [DataMember]
        public PhysicsType PhysicsType { get; private set; }

        [DataMember]
        public float Density { get; private set; }

        /*
            TODO: Decouple body from PhysicsComponent. Our view of PhysicsComponent is really a "Fixture" instead of a "Body"
        */

        [IgnoreDataMember]
        public Body Body { get; private set; }

        [DataMember]
        public CollisionGroup CollisionGroup { get; private set; }

        [DataMember]
        public CollisionGroup CollidesWith { get; private set; }

        [IgnoreDataMember]
        public Fixture Fixture { get; private set; }

        [IgnoreDataMember]
        public DxRectangle Bounds => new DxRectangle(0, 0, bounds_.X, bounds_.Y);

        [IgnoreDataMember]
        public DxVector2 Position
        {
            get { return Body?.Position ?? DxVector2.EmptyVector; }
            set
            {
                // TODO: Remove set?
                Body.Position = value.Vector2;
            }
        }

        [IgnoreDataMember]
        public DxRectangle Space => Bounds + Position;

        [IgnoreDataMember]
        public DxVector2 Center => Space.Center;

        private PhysicsComponent(DxVector2 origin, DxVector2 bounds, PhysicsType physicsType,
            CollisionGroup collidesWith, CollisionGroup collisionGroup, float density, bool gravityOn, float restitution, float friction)
        {
            origin_ = origin;
            bounds_ = bounds;
            gravity_ = gravityOn;
            restitution_ = restitution;
            friction_ = friction;
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
            Logger.Debug("Handling force: {0}", force);
            Body.ApplyForce(force.Value.Vector2, Body.WorldCenter);
        }

        private void HandleImpulseAttachment(Core.Physics.Impulse impulse)
        {
            Logger.Debug("Handling impulse: {0}", impulse);
            Body.LinearVelocity = impulse.Value.Vector2;
            //Body.ApplyLinearImpulse(impulse.Value.Vector2, Body.WorldCenter);
        }

        private void HandleNullificationAttachment(Nullification nullification)
        {
            Logger.Debug("Handling nullification: {0}", nullification);
            DxVector2 negationVelocity = DxVector2.EmptyVector;
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
            World gameWorld = DxGame.Instance.Model<CollisionModel>().World;

            PolygonShape bounds = new PolygonShape(Bounds.Vertices(), Density);
            Body = new Body(gameWorld, origin_.Vector2) {BodyType = ResolveCollisionType(PhysicsType)};
            if(!gravity_)
            {
                Body.IgnoreGravity = true;
            }

            Fixture = Body.CreateFixture(bounds, this);
            Fixture.CollidesWith = CollisionGroup.CollisionCategory;
            Body.Restitution = restitution_;
            Body.Friction = friction_;
            base.Initialize();
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
            private PhysicsType? physicsType_;
            private bool gravity_ = true;
            private float restitution_ = 0f;
            private float friction_ = 1f;
            private CollisionGroup collisionGroup_ = CollisionGroup.All;
            private CollisionGroup collidesWith_ = CollisionGroup.All;

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

            public PhysicsComponentBuilder WithRestitution(float restitution = 1f)
            {
                restitution_ = restitution;
                return null;
            }

            public PhysicsComponentBuilder WithoutFriction()
            {
                return WithFriction(0);
            }

            public PhysicsComponentBuilder WithFriction(float friction = 1f)
            {
                friction_ = friction;
                return this;
            }

            public PhysicsComponentBuilder WithDensity(float density)
            {
                density_ = density;
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

            public PhysicsComponentBuilder WithCollisionType(PhysicsType physicsType)
            {
                physicsType_ = physicsType;
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

            public PhysicsComponentBuilder WithoutCollision()
            {
                WithCollisionGroup(CollisionGroup.None);
                WithCollidesWith(CollisionGroup.None);
                return this;
            }

            public PhysicsComponent Build()
            {
                Validate.Hard.IsNotNullOrDefault(position_);
                Validate.Hard.IsNotNullOrDefault(bounds_);
                Validate.Hard.IsNotNullOrDefault(physicsType_);
                Validate.Hard.IsNotNegative(density_);
                Validate.Hard.IsNotNullOrDefault(collisionGroup_);
                Validate.Hard.IsNotNullOrDefault(collidesWith_);

                DxVector2 position = position_.Value;
                DxVector2 bounds = bounds_.Value;
                PhysicsType physicsType = physicsType_.Value;

                return new PhysicsComponent(position, bounds, physicsType, collidesWith_, collisionGroup_, density_,
                    gravity_, restitution_, friction_);
            }
        }
    }
}
