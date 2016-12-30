using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Properties;
using DxCore.Core.Utils;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Properties
{
    /**
        <summary>
            Answers the question "What do I do with all of these properties when I levelup?"
        </summary>
    */

    public delegate void LevelUpResponse(EntityProperties properties, LeveledUpMessage levelUpNotification);

    [Serializable]
    [DataContract]
    public class EntityPropertiesComponent : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly TimeSpan FORCED_NOTIFICATION_TRIGGER_DELAY = TimeSpan.FromSeconds(1 / 2.0);

        [DataMember] private TimeSpan lastTriggerNotification_;

        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        [IgnoreDataMember] private List<IProperty> properties_;

        [DataMember]
        public EntityProperties EntityProperties { get; }

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

        protected virtual DxVector2 InitialJumpAcceleration => DxVector2.EmptyVector;

        [DataMember]
        protected LevelUpResponse LevelUpResponse { get; }

        public EntityPropertiesComponent(EntityProperties entityProperties, LevelUpResponse levelUpResponse)
        {
            Validate.Hard.IsNotNull(entityProperties, this.GetFormattedNullOrDefaultMessage(entityProperties));
            Validate.Hard.IsNotNullOrDefault(levelUpResponse, this.GetFormattedNullOrDefaultMessage(levelUpResponse));
            EntityProperties = entityProperties;
            LevelUpResponse = levelUpResponse;
        }

        public override void Initialize()
        {
            /* Assume child class has dealt with actually creating the Properties */
            EntityProperties.Health.AttachListener(EntityDeathListener);
        }

        /* Takes your sweet properties and the level up notification and does absolutely nothing with them. */

        public static void NullLevelUpResponse(EntityProperties entityProperties, LeveledUpMessage levelUpMessage) {}

        public override void OnAttach()
        {
            RegisterMessageHandler<LeveledUpMessage>(HandleLevelUp);
        }

        protected virtual void EntityDeathListener(int previousHealth, int currentHealth)
        {
            /* Have we received lethal damage? */
            if((currentHealth <= 0) && (previousHealth > 0))
            {
                /* If so, tell everyone that we've died. */
                EntityDeathMessage entityDeathMessage = new EntityDeathMessage {Entity = Parent};
                /* The world deserves to know. We were important. */
                entityDeathMessage.Emit();
            }
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

        private void HandleLevelUp(LeveledUpMessage levelUp)
        {
            var leveledUpEntity = levelUp.Entity;
            if(Objects.Equals(Parent, leveledUpEntity))
            {
                LevelUpResponse(EntityProperties, levelUp);
            }
        }
    }
}