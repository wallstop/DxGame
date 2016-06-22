using System;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Advanced.Triggers;
using DxCore.Core.Properties;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NLog;

namespace Babel.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Pandora's Box is a last-chance item. Activates when the owner's Health falls beneath some threshold.
            Rapidly regenates the owner's Health up to 50%. 45 second cooldown (to be tweaked)
        </summary>

        <description>
            It's already been opened... all that's left is Hope.
        </description>
    */

    [DataContract]
    [Serializable]
    public class PandorasBox : ItemComponent
    {
        [DataMember]
        private AttachedPandorasBox AttachedPandorasBox { get; set; }

        protected override void InternalAttach(GameObject parent)
        {
            if(AttachedPandorasBox == null)
            {
                TimeSpan cooldown = TimeSpan.FromSeconds(45);
                const double triggerThreshold = .2;

                EntityPropertiesComponent entityPropertiesComponent =
                    parent.ComponentOfType<EntityPropertiesComponent>();
                EntityProperties properties = entityPropertiesComponent.EntityProperties;

                /* Simply create one - it will sit and listen for events on this player */
                AttachedPandorasBox = new AttachedPandorasBox(cooldown, triggerThreshold, parent, properties);
            }
            else
            {
                AttachedPandorasBox.IncreaseStackCount();
            }
        }

        protected override void InternalDetach(GameObject parent)
        {
            Validate.Hard.IsNotNullOrDefault(AttachedPandorasBox);
            AttachedPandorasBox.DecreaseStackCount();
        }
    }

    /**
        <summary>
            Network-serializable object that represents the "Attached" Pandora's Box to the player
        </summary>
    */

    [DataContract]
    [Serializable]
    internal sealed class AttachedPandorasBox
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly EntityProperties playerProperties_;

        [DataMember] private readonly GameObject source_;

        [DataMember] private readonly TimeSpan triggerDelay_;

        [DataMember] private readonly double triggerThreshold_;

        [DataMember] private Optional<TimeSpan> lastTriggered_;

        [DataMember]
        private int StackCount { get; set; } = 1;

        [DataMember]
        public bool Active { get; set; }

        private int MaxHealth => playerProperties_.MaxHealth.CurrentValue;

        public AttachedPandorasBox(TimeSpan triggerDelay, double triggerThreshold, GameObject source,
            EntityProperties playerProperties)
        {
            Validate.Hard.IsInOpenInterval(triggerThreshold, 0, 1,
                $"Cannot create an {typeof(AttachedPandorasBox)} with a {nameof(triggerThreshold)} of {triggerThreshold})");
            Validate.Hard.IsNotNull(source, this.GetFormattedNullOrDefaultMessage(source));
            Validate.Hard.IsNotNull(playerProperties, this.GetFormattedNullOrDefaultMessage(nameof(playerProperties)));
            source_ = source;
            triggerDelay_ = triggerDelay;
            triggerThreshold_ = triggerThreshold;
            playerProperties_ = playerProperties;
            lastTriggered_ = Optional<TimeSpan>.Empty;
            playerProperties_.Health.AttachListener(CheckForTrigger);

            Active = false;
        }

        public void IncreaseStackCount()
        {
            ++StackCount;
        }

        public void DecreaseStackCount()
        {
            Validate.Hard.IsTrue(StackCount > 0,
                $"Cannot decrease the stack count of a {nameof(AttachedPandorasBox)} below 0!");
            --StackCount;
        }

        public void CheckForTrigger(int previous, int current)
        {
            /* If the new value doesn't trigger us, nothing to do, bail */
            if(current > Math.Round(triggerThreshold_ * MaxHealth))
            {
                return;
            }

            if(Active)
            {
                LOG.Info(
                    $"Received trigger event for health transition [{previous}->{current}], but was already activate (ignoring)");
                return;
            }

            TimeSpan currentTime = DxGame.Instance.CurrentTime.TotalGameTime;
            if(!lastTriggered_.HasValue || lastTriggered_.Value + triggerDelay_ <= currentTime)
            {
                lastTriggered_ = Optional<TimeSpan>.Of(currentTime);
                Active = true;
                Trigger();
            }
        }

        private void Trigger()
        {
            const int durationSeconds = 10;
            const int numTicks = 10;

            TimeSpan duration = TimeSpan.FromSeconds(durationSeconds);
            TimeSpan tickRate = TimeSpan.FromSeconds((double) durationSeconds / numTicks);

            const double baseTargetPercentage = .5;
            const double maxTargetPercentage = 1.5;
            const int maxStacks = 100;

            double targetHealthPercentage = SpringFunctions.ExponentialEaseOutIn(baseTargetPercentage,
                maxTargetPercentage, Math.Min(StackCount, maxStacks), maxStacks);

            int amountToHeal =
                (int)
                    Math.Round(playerProperties_.MaxHealth.CurrentValue * targetHealthPercentage -
                               playerProperties_.Health.CurrentValue);

            AttachedHealer attachedHealer = new AttachedHealer(amountToHeal, numTicks);

            TimedTriggeredActionComponent<EntityProperties> pandoraHealthRegen =
                new TimedTriggeredActionComponent<EntityProperties>(duration, tickRate, playerProperties_,
                    attachedHealer.Tick, properties => Active = false);
            pandoraHealthRegen.Create();
            source_.AttachComponent(pandoraHealthRegen);
        }
    }

    /**
        <summary>
            Network-serializable healer that gets created on Pandora's Box trigger (heals for some duration)
        </summary>
    */

    [DataContract]
    [Serializable]
    internal sealed class AttachedHealer
    {
        [DataMember] private int amountHealed_;

        [DataMember] private int ticks_;

        [DataMember]
        private int AmountToHeal { get; }

        [DataMember]
        private int NumTicks { get; }

        private double HealPerTick => AmountToHeal / (1.0 * NumTicks);

        public AttachedHealer(int amountToHeal, int numTicks)
        {
            Validate.Hard.IsTrue(amountToHeal >= 0,
                $"Cannot create an {typeof(AttachedHealer)} with an {nameof(amountToHeal)} of {amountToHeal}");
            AmountToHeal = amountToHeal;
            Validate.Hard.IsTrue(numTicks > 0,
                $"Cannot create an {typeof(AttachedHealer)} with a {nameof(numTicks)} of {numTicks}");
            NumTicks = numTicks;
        }

        public void Tick(EntityProperties entityProperties)
        {
            ++ticks_;
            Validate.Hard.IsTrue(ticks_ <= NumTicks, "Ticked too many times!");

            int target = (int) Math.Round(ticks_ * HealPerTick);
            int amountToHeal = Math.Max(0, target - amountHealed_);

            entityProperties.Health.BaseValue += amountToHeal;
            amountHealed_ += amountToHeal;
        }
    }
}