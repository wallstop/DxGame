using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced.Triggers
{
    /**

        <summary>
            Encapsulates the question "Has this thing happened yet?"
        </summary>
    */

    public delegate bool Trigger();

    /**

        <summary>
            Waits for some condition to occur (specified by Trigger), then executes the provided Action and removes itself from the game.
        </summary>
    */

    /* TODO: Fully replace with TriggeredActionComponent */

    [Serializable]
    [DataContract]
    public class TriggerComponent : Component
    {
        private TimeSpan lastChecked_;

        [DataMember]
        private Trigger Trigger { get; }

        /* Executed when the trigger returns true */

        [DataMember]
        private Action Action { get; }

        [DataMember]
        private TimeSpan CheckFrequency { get; }

        public TriggerComponent(Trigger trigger, Action action, TimeSpan checkFrequency = new TimeSpan())
        {
            Validate.IsNotNull(trigger, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(trigger)));
            Validate.IsNotNull(action, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(action)));
            Trigger = trigger;
            Action = action;
            lastChecked_ = TimeSpan.Zero;
            CheckFrequency = checkFrequency;
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(!(lastChecked_ + CheckFrequency < gameTime.TotalGameTime))
            {
                return;
            }

            lastChecked_ = gameTime.TotalGameTime;
            bool triggered = Trigger();
            if(triggered)
            {
                Action();
                Dispose();
            }
        }
    }
}