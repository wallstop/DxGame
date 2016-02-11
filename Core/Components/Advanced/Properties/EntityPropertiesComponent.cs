using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Properties
{
    /**
        <summary>
            Answers the question "What do I do with all of these properties when I levelup?"
        </summary>
    */

    public delegate void LevelUpResponse(EntityProperties properties, LeveledUpMessage levelUpNotification);

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class EntityPropertiesComponent : Component
    {
        private static readonly TimeSpan FORCED_NOTIFICATION_TRIGGER_DELAY = TimeSpan.FromSeconds(1 / 2.0);

        [DataMember] [ProtoMember(1)] private TimeSpan lastTriggerNotification_;

        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        [IgnoreDataMember] private List<IProperty> properties_;

        /* TODO: Move into EntityProperties */

        public IEnumerable<IProperty> Properties
        {
            get
            {
                if(!ReferenceEquals(properties_, null))
                {
                    return properties_;
                }
                PropertyInfo[] entityProperties =
                    typeof(EntityProperties).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                List<IProperty> properties = new List<IProperty>(entityProperties.Length);
                foreach(var entityProperty in entityProperties)
                {
                    if(!typeof(IProperty).IsAssignableFrom(entityProperty.PropertyType))
                    {
                        continue;
                    }

                    IProperty property = (IProperty) entityProperty.GetValue(EntityProperties);
                    properties.Add(property);
                }
                properties_ = properties;
                return properties_;
            }
        }

        [DataMember]
        [ProtoMember(2)]
        public EntityProperties EntityProperties { get; }

        [DataMember]
        [ProtoMember(3)]
        protected LevelUpResponse LevelUpResponse { get; }

        protected virtual DxVector2 InitialJumpAcceleration => DxVector2.EmptyVector;

        public EntityPropertiesComponent(EntityProperties entityProperties, LevelUpResponse levelUpResponse)
        {
            Validate.IsNotNull(entityProperties, StringUtils.GetFormattedNullOrDefaultMessage(this, entityProperties));
            Validate.IsNotNullOrDefault(levelUpResponse,
                StringUtils.GetFormattedNullOrDefaultMessage(this, levelUpResponse));
            EntityProperties = entityProperties;
            LevelUpResponse = levelUpResponse;
            MessageHandler.RegisterMessageHandler<LeveledUpMessage>(HandleLevelUp);
        }

        public override void Initialize()
        {
            /* Assume child class has dealt with actually creating the Properties */
            EntityProperties.Health.AttachListener(EntityDeathListener);
        }

        protected override void Update(DxGameTime gameTime)
        {
            TimeSpan currentTime = gameTime.TotalGameTime;
            if(currentTime <= lastTriggerNotification_ + FORCED_NOTIFICATION_TRIGGER_DELAY)
            {
                return;
            }

            lastTriggerNotification_ = currentTime;
            foreach(IProperty property in Properties)
            {
                property.TriggerListeners();
            }
        }

        /* Takes your sweet properties and the level up notification and does absolutely nothing with them. */

        public static void NullLevelUpResponse(EntityProperties entityProperties, LeveledUpMessage levelUpMessage) {}

        private void HandleLevelUp(LeveledUpMessage levelUp)
        {
            var leveledUpEntity = levelUp.Entity;
            if(Objects.Equals(Parent, leveledUpEntity))
            {
                LevelUpResponse(EntityProperties, levelUp);
            }
        }

        public virtual Force MovementForceFor(Commandment commandment)
        {
            switch(commandment)
            {
                case Commandment.MoveLeft:
                    return MoveLeftForce();
                case Commandment.MoveRight:
                    return MoveRightForce();
                case Commandment.MoveUp:
                    return JumpForce();
                default:
                    return Force.NullForce;
            }
        }

        protected virtual Force MoveLeftForce()
        {
            var movement = new Movement(new DxVector2(-EntityProperties.MoveSpeed.CurrentValue, 0), "MoveLeft");
            return movement.Force;
        }

        protected virtual Force MoveRightForce()
        {
            var movement = new Movement(new DxVector2(EntityProperties.MoveSpeed.CurrentValue, 0), "MoveRight");
            return movement.Force;
        }

        protected virtual Force JumpForce()
        {
            var physics = Parent.ComponentOfType<PhysicsComponent>();
            var currentVelocity = physics.Velocity;
            physics.Velocity = new DxVector2(currentVelocity.X, Math.Min(0, currentVelocity.Y));
            var initialVelocity = new DxVector2(0, -EntityProperties.JumpSpeed.CurrentValue * 1.6);
            var force = new Force(initialVelocity, InitialJumpAcceleration, JumpDissipation(), "Jump");
            return force;
        }

        protected virtual void EntityDeathListener(int previousHealth, int currentHealth)
        {
            /* Have we received lethal damage? */
            if(currentHealth <= 0 && previousHealth > 0)
            {
                /* If so, tell everyone that we've died. */
                var entityDeathMessage = new EntityDeathMessage {Entity = Parent};
                /* The world deserves to know. We were important. */
                DxGame.Instance.BroadcastMessage(entityDeathMessage);
                Parent?.BroadcastMessage(entityDeathMessage);
            }
        }

        protected virtual DissipationFunction JumpDissipation()
        {
            DissipationFunction dissipation = (externalVelocity, acceleration, gameTime) =>
            {
                /* If our jumping force is applying a (down into the ground) acceleration, we're done! */
                var done = externalVelocity.Y >= 0;
                if(done)
                {
                    return Tuple.Create(true, new DxVector2());
                }
                return Tuple.Create(false, acceleration);
            };
            return dissipation;
        }
    }

    [Serializable]
    [DataContract]
    [ProtoContract]
    internal sealed class Movement
    {
        [DataMember] [ProtoMember(1)] private bool dissipated_;

        [DataMember]
        [ProtoMember(2)]
        public Force Force { get; }

        [DataMember]
        [ProtoMember(3)]
        private DxVector2 Direction { get; }

        public Movement(DxVector2 directionalForceVector, string forceName)
        {
            Direction = directionalForceVector;
            Force = new Force(DxVector2.EmptyVector, directionalForceVector, DissipationFunction, forceName);
        }

        /* Honestly I do not remember wtf any of this is, seems pretty complicated & cool */

        private Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration,
            DxGameTime gameTime)
        {
            var xDirectionSign = Math.Sign(Direction.X);
            var xIsNegative = xDirectionSign == -1;
            DxVector2 accelerationVector = new DxVector2
            {
                X =
                    xIsNegative
                        ? Math.Min(Direction.X - externalVelocity.X, 0)
                        : Math.Max(Direction.X - externalVelocity.X, 0)
            };
            accelerationVector.X = xIsNegative
                ? Math.Max(accelerationVector.X, Direction.X)
                : Math.Min(accelerationVector.X, Direction.X);

            var yDirectionSign = Math.Sign(Direction.Y);
            var yIsNegative = yDirectionSign == -1;

            accelerationVector.Y = yIsNegative
                ? Math.Min(Direction.Y - externalVelocity.Y, 0)
                : Math.Max(Direction.Y - externalVelocity.Y, 0);
            accelerationVector.Y = yIsNegative
                ? Math.Max(accelerationVector.Y, Direction.Y)
                : Math.Min(accelerationVector.Y, Direction.Y);
            var hasDissipated = dissipated_;
            // Dissipate after one frame always (we need continual move left requests to actually move)
            dissipated_ = true;
            return Tuple.Create(hasDissipated, accelerationVector);
        }
    }
}