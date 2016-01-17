using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Properties;

namespace DXGame.TowerGame.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Fragarach is an attack speed steroid
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

        public Fragarach(SpatialComponent spatial) : base(spatial) {}

        private static int FragarachIncreasedAttackSpeed(int originalAttackSpeed)
        {
            const double scaleFactor = 1.3; // (30% increased attack speed)
            return (int) Math.Round(originalAttackSpeed * scaleFactor);
        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            bool relevant = CheckIsRelevantEnvironmentInteraction(environmentInteraction);
            if(!relevant)
            {
                return;
            }

            Activated = true;
            GameObject source = environmentInteraction.Source;
            EntityProperties playerProperties = source.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            PropertyMutator<int> fragarachAttackSpeedBuff = FRAGARACH_ATTACK_SPEED_BUFF;
            playerProperties.AttackSpeed.AddMutator(fragarachAttackSpeedBuff);

            Dispose();
        }
    }
}