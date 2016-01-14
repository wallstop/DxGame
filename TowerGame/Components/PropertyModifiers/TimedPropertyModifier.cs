using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.TowerGame.Components.PropertyModifiers
{
    public delegate void PropertyModifier(EntityProperties entityProperties);

    /**
        <summary>
            Addresses the need for "I want to do this thing continually, every n time units, until m time units have elapsed".

            In particular, this is useful for things like heal or damage over time.
        </summary>
    */

    [DataContract]
    [Serializable]
    public class TimedPropertyModifier<T> : Component
    {
        [DataMember] private readonly EntityProperties entityProperties_;

        [DataMember] private readonly PropertyModifier propertyModifier_;

        [DataMember] private TimeSpan lastTick_;

        [DataMember]
        public TimeSpan Duration { get; }

        [DataMember]
        public TimeSpan TickRate { get; }

        [DataMember]
        public TimeSpan Initialized { get; }

        public TimedPropertyModifier(TimeSpan duration, TimeSpan tickRate, EntityProperties entityProperties,
            PropertyModifier propertyModifier)
        {
            Validate.IsTrue(tickRate <= duration,
                $"Cannot create a {typeof(TimedPropertyModifier<T>)} with a {nameof(duration)} ({duration}) less than {nameof(tickRate)} ({tickRate}). (Arguments reversed?)");
            Validate.IsTrue(tickRate > TimeSpan.Zero,
                $"Cannot create a {typeof(TimedPropertyModifier<T>)} with a {nameof(tickRate)} of Zero");
            Validate.IsNotNullOrDefault(entityProperties,
                StringUtils.GetFormattedNullOrDefaultMessage(this, entityProperties));
            Validate.IsNotNull(propertyModifier, StringUtils.GetFormattedNullOrDefaultMessage(this, propertyModifier));

            Duration = duration;
            TickRate = tickRate;
            entityProperties_ = entityProperties;
            propertyModifier_ = propertyModifier;
            Initialized = DxGame.Instance.CurrentTime.ElapsedGameTime;
            lastTick_ = Initialized;
        }

        protected override void Update(DxGameTime gameTime)
        {
            while(lastTick_ + TickRate <= gameTime.TotalGameTime)
            {
                lastTick_ += TickRate;
                propertyModifier_.Invoke(entityProperties_);
            }

            if(Initialized + Duration < gameTime.TotalGameTime)
            {
                Dispose();
            }
        }
    }
}