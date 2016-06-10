using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced.Triggers
{
    /**
        <summary>
            Addresses the need for "I want to do this thing continually, every n time units, until m time units have elapsed".

            In particular, this is useful for things like heal or damage over time.
        </summary>
    */

    [DataContract]
    [Serializable]
    public class TimedTriggeredActionComponent<T> : TriggeredActionComponent<T>
    {
        public TimedTriggeredActionComponent(TimeSpan duration, TimeSpan tickRate, T source, Action<T> action)
            : base(new EndTrigger(duration).IsEnded, new TickRate(tickRate).DetermineNumTicks, source, action) {}

        public TimedTriggeredActionComponent(TimeSpan duration, TimeSpan tickRate, T source, Action<T> action,
            Action<T> finalAction)
            : base(
                new EndTrigger(duration).IsEnded, new TickRate(tickRate).DetermineNumTicks, source, action, finalAction) {}
    }

    /**
        TODO: Refactor somehow
    */

    [DataContract]
    [Serializable]
    public class TimedTriggeredActionComponent : TriggeredActionComponent
    {
        public TimedTriggeredActionComponent(TimeSpan duration, TimeSpan tickRate, Action action)
            : base(new EndTrigger(duration).IsEnded, new TickRate(tickRate).DetermineNumTicks, action) {}

        public TimedTriggeredActionComponent(TimeSpan duration, TimeSpan tickRate, Action action, Action finalAction)
            : base(new EndTrigger(duration).IsEnded, new TickRate(tickRate).DetermineNumTicks, action, finalAction) {}
    }

    [DataContract]
    [Serializable]
    internal sealed class TickRate
    {
        [DataMember] private TimeSpan lastTicked_;

        [DataMember]
        private TimeSpan Rate { get; }

        public TickRate(TimeSpan tickRate)
        {
            Validate.IsNotNullOrDefault(tickRate, this.GetFormattedNullOrDefaultMessage(nameof(tickRate)));
            Rate = tickRate;
            /* TODO: Find a better way of passing in "last ticked". Maybe we want to immediately tick? */
            lastTicked_ = DxGame.Instance.CurrentTime.TotalGameTime;
        }

        public int DetermineNumTicks(DxGameTime gameTime)
        {
            int numTicks = 0;
            while(lastTicked_ + Rate <= gameTime.TotalGameTime)
            {
                ++numTicks;
                lastTicked_ += Rate;
            }
            return numTicks;
        }
    }

    [DataContract]
    [Serializable]
    internal sealed class EndTrigger
    {
        [DataMember]
        private TimeSpan Duration { get; }

        public EndTrigger(TimeSpan duration)
        {
            Duration = duration;
        }

        public bool IsEnded(TimeSpan initial, DxGameTime current)
        {
            return initial + Duration < current.TotalGameTime;
        }
    }
}