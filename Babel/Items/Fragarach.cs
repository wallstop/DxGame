using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Properties;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace Babel.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Fragarach is an attack speed steroid

            TODO: Implement increased damage to dragooooons
        </summary>
        <description>
            The wielder of this blade becomes as swift as the wind.
        </description>
    */

    [DataContract]
    [Serializable]
    public class Fragarach : ItemComponent
    {
        private static readonly PropertyMutator<int> FRAGARACH_ATTACK_SPEED_BUFF =
            new PropertyMutator<int>(FragarachIncreasedAttackSpeed, "Fragarach");

        private static int FragarachIncreasedAttackSpeed(int originalAttackSpeed, int fragarachCount)
        {
            /* TODO: Pull out this scaling into somewhere more common (maybe?) */
            const int maxFragarachStacks = 100;

            const double baseAttackSpeedIncrease = .3;
            const double maxAttackSpeedIncrease = 2.0;
            double fragarachScaleFactor = SpringFunctions.ExponentialEaseOutIn(baseAttackSpeedIncrease,
                maxAttackSpeedIncrease, Math.Min(maxFragarachStacks, fragarachCount), maxFragarachStacks);

            return (int) Math.Round(originalAttackSpeed * (1.0 + fragarachScaleFactor));
        }

        protected override void InternalAttach(GameObject parent)
        {
            EntityProperties playerProperties = parent.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            playerProperties.AttackSpeed.AddMutator(FRAGARACH_ATTACK_SPEED_BUFF);
        }

        protected override void InternalDetach(GameObject parent)
        {
            EntityProperties playerProperties = parent.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            playerProperties.AttackSpeed.RemoveMutator(FRAGARACH_ATTACK_SPEED_BUFF);
        }
    }
}