using System;
using System.Collections.Generic;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Player;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Skills;
using DxCore.Core.State;
using DXGame.Core;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Generators
{
    public class PlayerGenerator
    {
        private readonly FloatingHealthIndicator healthBar_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;
        public SpatialComponent PlayerSpace { get; }

        public PlayerGenerator(DxVector2 playerPosition)
        {
            PlayerSpace = new MapBoundedSpatialComponent(playerPosition, new DxVector2(75, 75));
            physics_ =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(PlayerSpace).Build();

            playerProperties_ = new EntityPropertiesComponent(PlayerFactory.BasicPlayerProperties,
                PlayerFactory.GenericLevelUp);
            /* Fuck with the health so we can check if the hp bar works */
            playerProperties_.EntityProperties.Health.BaseValue -= 3;
            // TODO: Need to add state machine in (how?)

            // TODO make these colors not shit and/or blend into the current biome
            healthBar_ =
                FloatingHealthIndicator.Builder()
                    .WithForegroundColor(Color.Green)
                    .WithEntityProperties(playerProperties_)
                    .WithPosition(PlayerSpace)
                    .Build();
            healthBar_.LoadContent();
        }

        public GameObject GeneratePlayer(AbstractCommandComponent playerCommander, bool isActivePlayer)
        {
            GameObject.GameObjectBuilder playerBuilder = GameObject.Builder();
            FacingComponent facingComponent = new FacingComponent();
            TeamComponent teamComponent = new TeamComponent(Team.PlayerTeam);
            LevelComponent levelComponent = new LevelComponent();
            BasicAttackComponent basicAttackListener = new BasicAttackComponent();
            GevurahBasicAttack gevurahBasicAttack = new GevurahBasicAttack();
            ItemManager itemManager = new ItemManager();

            string playerName = "rektorOfSouls";
            PlayerNameComponent playerNameComponent = new PlayerNameComponent(playerName);
            if(isActivePlayer)
            {
                ActivePlayerComponent activePlayerComponent = new ActivePlayerComponent();
                playerBuilder.WithComponent(activePlayerComponent);
            }
            playerBuilder.WithComponents(PlayerSpace, physics_, playerProperties_, healthBar_, playerCommander,
                facingComponent, teamComponent, levelComponent, basicAttackListener, gevurahBasicAttack, itemManager,
                playerNameComponent);
            GameObject playerObject = playerBuilder.Build();
            Skill shockwaveSkill =
                Skill.Builder()
                    .WithCooldown(TimeSpan.FromSeconds(1))
                    .WithSkillFunction(Gevurah.Shockwave)
                    .WithCommandment(Commandment.Ability1)
                    .Build();
            Skill arrowRainSkill =
                Skill.Builder()
                    .WithCooldown(TimeSpan.FromSeconds(5))
                    .WithSkillFunction(Gevurah.RainOfArrows)
                    .WithCommandment(Commandment.Ability2)
                    .Build();
            Skill chargeShot =
                ChargedSkill.Builder()
                    .WithCooldown(TimeSpan.FromSeconds(5))
                    .WithSkillFunction(Gevurah.ChargeShot)
                    .WithCommandment(Commandment.Ability4)
                    .Build();
            Skill archerRoll =
                Skill.Builder()
                    .WithCooldown(TimeSpan.FromSeconds(3))
                    .WithSkillFunction(Gevurah.BearTrapRoll)
                    .WithCommandment(Commandment.Movement)
                    .Build();

            SkillComponent playerSkillComponent = new SkillComponent(shockwaveSkill, arrowRainSkill, chargeShot,
                archerRoll);
            playerObject.AttachComponent(playerSkillComponent);

            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(playerObject, "Poison");
            return playerObject;
        }

        public GameObject GeneratePlayer()
        {
            return GeneratePlayer(new PlayerInputListener(), true);
        }

        public List<GameObject> Generate()
        {
            return new List<GameObject> {GeneratePlayer()};
        }
    }
}