using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Advanced.Triggers;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            TheFirstFlame is an attack modifier - has a chance to activate for each enemy hit
        </summary>
        <description>
            The first flame brought to Earth by a Titan still burns strong.
        </description>
    */

    [DataContract]
    [Serializable]
    public class TheFirstFlame : ItemComponent
    {
        private static readonly float TRIGGER_THRESHOLD = 0.2f;

        public TheFirstFlame(SpatialComponent spatial) : base(spatial)
        {
            MessageHandler.RegisterMessageHandler<AttackBuilder>(HandleAttackBuilderRequest);
        }

        protected void HandleAttackBuilderRequest(AttackBuilder attackBuilder)
        {
            attackBuilder.WithPhysicsMessageGenerator(Burninator);
        }

        private PhysicsMessage Burninator(GameObject source, ICollection<IShape> sourceAttackAreas)
        {
            PhysicsMessage possibleBurnInteraction = new PhysicsMessage
            {
                AffectedAreas = sourceAttackAreas.ToList(),
                Source = Parent,
                Interaction = BurnTrigger
            };
            return possibleBurnInteraction;
        }

        private void BurnTrigger(GameObject source, PhysicsComponent destination)
        {
            TeamComponent teamComponent = destination.Parent.ComponentOfType<TeamComponent>();
            TeamComponent ourTeam = Parent.ComponentOfType<TeamComponent>();
            /* Same team? No friendly fire */
            // TODO: Consolidate friendly fire check / these general checks into... somewhere else
            if(Objects.Equals(ourTeam.Team, teamComponent.Team))
            {
                return;
            }

            /* Check random trigger chance */
            float rng = ThreadLocalRandom.Current.NextFloat();
            if(rng > TRIGGER_THRESHOLD)
            {
                return;
            }
            EntityPropertiesComponent entityPropertiesComponent =
                destination.Parent.ComponentOfType<EntityPropertiesComponent>();
            Component burninator = GenerateBurnDamage(entityPropertiesComponent.EntityProperties);
            destination.Parent.AttachComponent(burninator);
        }

        private static TimedTriggeredActionComponent<EntityProperties> GenerateBurnDamage(EntityProperties properties)
        {
            TimeSpan duration = TimeSpan.FromSeconds(1);
            TimeSpan tickRate = TimeSpan.FromSeconds(0.5);
            const int damage = 1;
            TimedTriggeredActionComponent<EntityProperties> burnTicker =
                new TimedTriggeredActionComponent<EntityProperties>(duration, tickRate, properties,
                    entityProperties => entityProperties.Health.BaseValue -= damage);
            return burnTicker;
        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}