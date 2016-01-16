using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Properties;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public class Gram : ItemComponent
    {
        private static readonly PropertyMutator<int> GRAM_DAMAGE_BUFF = new PropertyMutator<int>(GramIncreasedDamage,
            "Gram");

        public Gram(SpatialComponent spatial) : base(spatial) {}

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
            PropertyMutator<int> gramDamageBuff = GRAM_DAMAGE_BUFF;
            playerProperties.AttackDamage.AddMutator(gramDamageBuff);

            Dispose();
        }

        private static int GramIncreasedDamage(int originalDamage)
        {
            double scaleFactor = 1.3; // (130% increased damage)
            return (int) Math.Round(originalDamage * scaleFactor);
        }
    }
}