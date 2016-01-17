﻿using System;
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

            Gram is a simple damage buff.
            TODO: Figure out how to add ... specific triggers when specific events happen? (Dealing damage to dragons, for example)
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
            const double scaleFactor = 1.3; // (30% increased damage)
            return (int) Math.Round(originalDamage * scaleFactor);
        }
    }
}