using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using NLog;
using ProtoBuf;
using Component = DXGame.Core.Components.Basic.Component;

namespace DXGame.TowerGame.Skills.Gevurah
{
    [DataContract]
    [Serializable]
    [ProtoContract]
    public class GevurahBasicAttack : Component
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public GevurahBasicAttack()
        {
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<AttackRequest>(HandleAttackRequest);
        }

        private void HandleAttackRequest(AttackRequest attackRequest)
        {
            EntityProperties entityProperties = Parent.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            if(entityProperties == null)
            {
                LOG.Warn(
                    $"No {typeof(EntityPropertiesComponent)} found for {Parent}, ignoring request for {typeof(GevurahBasicAttack)}");
                return;
            }

            DxRectangle attackArea = DetermineAttackArea();

            /* TODO: Need to deal with states / animation */
            AttackBuilder attackBuilder =
                new AttackBuilder(Parent, new List<IShape> {attackArea}).WithPhysicsMessageGenerator(GenerateBasicAttack);
            attackBuilder.Emit();
            List<PhysicsMessage> attacks = attackBuilder.Build();
            foreach(PhysicsMessage attack in attacks)
            {
                attack.Emit();
            }
        }

        private DxRectangle DetermineAttackArea()
        {
            FacingComponent facingComponent = Parent.ComponentOfType<FacingComponent>();
            SpatialComponent space = Parent.ComponentOfType<SpatialComponent>();
            if(facingComponent.Facing == Direction.East)
            {
                /* Attack area is our own area, shifted to the right */
                return new DxRectangle(space.Space.Right, space.Space.Top, space.Space.Width, space.Space.Height);
            }
            if(facingComponent.Facing == Direction.West)
            {
                return new DxRectangle(space.Space.Left - space.Space.Width, space.Space.Top, space.Space.Width,
                    space.Space.Height);
            }
            string errorMessage = $"Unable to determine proper attack area for direction {facingComponent.Facing}";
            LOG.Error(errorMessage);
            throw new InvalidEnumArgumentException(errorMessage);
        }

        private PhysicsMessage GenerateBasicAttack(GameObject source, ICollection<IShape> sourceAttackAreas)
        {
            PhysicsMessage attackedMessage = new PhysicsMessage
            {
                AffectedAreas = sourceAttackAreas.ToList(),
                Source = source,
                Interaction = AttackInteraction
            };
            return attackedMessage;
        }

        private void AttackInteraction(GameObject source, PhysicsComponent destination)
        {
            GameObject affectedEntity = destination.Parent;
            if(ReferenceEquals(affectedEntity, null))
            {
                return;
            }
            TeamComponent teamComponent = affectedEntity.ComponentOfType<TeamComponent>();
            if(!ReferenceEquals(teamComponent, null))
            {
                /* 
                    Cheat - we know we're on the player's team. Otherwise, we could retrieve the team from the source object 
                */

                if(Equals(teamComponent.Team, Team.PlayerTeam))
                {
                    return;
                }
            }

            /* If they either don't have a team component or are not on our team, blaze em */
            DamageMessage damage = new DamageMessage {DamageCheck = DamageCheck, Source = source, Target = destination.Parent?.Id};
            damage.Emit();
        }

        private Tuple<bool, double> DamageCheck(GameObject source, GameObject destination)
        {
            EntityProperties entityProperties = source.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            int attackDamage = entityProperties.AttackDamage.CurrentValue;
            return Tuple.Create(true, (double) attackDamage);
        }
    }
}