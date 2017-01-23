using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Triggers
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
        [DataMember] private TimeSpan lastChecked_;

        /* Executed when the trigger returns true */

        [DataMember]
        private Action Action { get; }

        [DataMember]
        private TimeSpan CheckFrequency { get; }

        [DataMember]
        private Trigger Trigger { get; }

        public TriggerComponent(Trigger trigger, Action action, TimeSpan checkFrequency = new TimeSpan())
        {
            Validate.Hard.IsNotNull(trigger, this.GetFormattedNullOrDefaultMessage(nameof(trigger)));
            Validate.Hard.IsNotNull(action, this.GetFormattedNullOrDefaultMessage(nameof(action)));
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
                Remove();
            }
        }
    }
}