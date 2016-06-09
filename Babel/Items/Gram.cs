using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Properties;
using DXGame.Core;
using DXGame.Core.Properties;
using DXGame.Core.Utils;

namespace Babel.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Gram is a simple damage buff.
        </summary>
        <description>
            A blade sharp enough to slay a dragon.
        </description>
    */

    [DataContract]
    [Serializable]
    public class Gram : ItemComponent
    {
        private static readonly PropertyMutator<int> GRAM_DAMAGE_BUFF = new PropertyMutator<int>(GramIncreasedDamage,
            "Gram");

        private static int GramIncreasedDamage(int originalDamage, int stackCount)
        {
            const int maxGramStacks = 100;

            const double maxDamageIncrease = 5.0;
            const double baseDamageIncrease = 0.3;

            double damageIncrease = SpringFunctions.ExponentialEaseOutIn(baseDamageIncrease, maxDamageIncrease,
                Math.Min(stackCount, maxGramStacks), maxGramStacks);

            return (int) Math.Round(originalDamage * (1.0 + damageIncrease));
        }

        protected override void InternalAttach(GameObject parent)
        {
            EntityProperties playerProperties = parent.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            playerProperties.AttackDamage.AddMutator(GRAM_DAMAGE_BUFF);
        }

        protected override void InternalDetach(GameObject parent)
        {
            EntityProperties playerProperties = parent.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            playerProperties.AttackDamage.RemoveMutator(GRAM_DAMAGE_BUFF);
        }
    }
}