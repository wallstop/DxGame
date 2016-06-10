using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace Babel.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Svalinn is a defensive item similar to Dota's Mjollnir active - it triggers chain lightening upon taking damage
        </summary>
        <description>
            A shield cold enough to withstand the fierce heat of the sun.
        </description>
    */

    [DataContract]
    [Serializable]
    public class Svalinn : ItemComponent
    {
        private void DamageCheckListener(int previousHealth, int currentHealth)
        {
            if(previousHealth <= currentHealth)
            {
                /* No actual damage taken - we only trigger on damage, so bail */
                return;
            }

            const double baseTriggerChance = 0.05;
            const double maxTriggerChance = 0.9;
            const int maxStacks = 100;

            double chance = SpringFunctions.ExponentialEaseOutIn(baseTriggerChance, maxTriggerChance,
                Math.Min(StackCount, maxStacks), maxStacks);

            float rng = ThreadLocalRandom.Current.NextFloat();
            if(rng > chance)
            {
                /* Didn't meat trigger requirements - bail */
                return;
            }

            /* Trigger chain lightening effect */

            // TODO: Chain and stuff (Emit Physics Message -> Damage message -> Repeat)

        }

        protected override void InternalAttach(GameObject parent)
        {
            EntityPropertiesComponent entityProperties = parent.ComponentOfType<EntityPropertiesComponent>();
            entityProperties.EntityProperties.Health.AttachListener(DamageCheckListener);
        }

        protected override void InternalDetach(GameObject parent)
        {
            EntityPropertiesComponent entityProperties = parent.ComponentOfType<EntityPropertiesComponent>();
            entityProperties.EntityProperties.Health.RemoveListener(DamageCheckListener);
        }
    }

    [Serializable]
    internal sealed class ChainLightning
    {
        //private static readonly int MAX_CHAINS = 6;

        //public 


    }
}
