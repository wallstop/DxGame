using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Advanced.Triggers;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Properties;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace Babel.Items
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
        public override void OnAttach()
        {
            RegisterMessageHandler<AttackBuilder>(HandleAttackBuilderRequest);
            base.OnAttach();
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

            const double baseTriggerThreshold = 0.15;
            const double maxTriggerThreshold = 0.85;
            const int maxStacks = 100;

            double threshold = SpringFunctions.ExponentialEaseOutIn(baseTriggerThreshold, maxTriggerThreshold,
                Math.Min(StackCount, maxStacks), maxStacks);

            /* Check random trigger chance */
            float rng = ThreadLocalRandom.Current.NextFloat();
            if(rng > threshold)
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

        protected override void InternalAttach(GameObject parent)
        {
            // No-op
        }

        protected override void InternalDetach(GameObject parent)
        {
            // No-op
        }
    }
}