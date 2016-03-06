using System;
using System.Collections.Generic;
using DXGame.Core.Behavior.Goals;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Player;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Skills;
using DXGame.Core.State;
using DXGame.Main;
using DXGame.TowerGame.Enemies;
using DXGame.TowerGame.Items;
using DXGame.TowerGame.Skills.Gevurah;
using Microsoft.Xna.Framework;
using DXGame.TowerGame.Player;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator
    {
        private readonly FloatingHealthIndicator healthBar_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;
        public SpatialComponent PlayerSpace { get; }

        public PlayerGenerator(DxVector2 playerPosition, DxRectangle bounds)
        {
            PlayerSpace = new MapBoundedSpatialComponent(playerPosition, new DxVector2(50, 50));
            physics_ =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(PlayerSpace).Build();

            playerProperties_ = new EntityPropertiesComponent(PlayerFactory.BasicPlayerProperties, PlayerFactory.GenericLevelUp);
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

        public List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var playerBuilder = GameObject.Builder();
            var inputListener = new PlayerInputListener();
            var facingComponent = new FacingComponent();
            var teamComponent = new TeamComponent(Team.PlayerTeam);
            LevelComponent levelComponent = new LevelComponent();
            BasicAttackComponent basicAttackListener = new BasicAttackComponent();
            GevurahBasicAttack gevurahBasicAttack = new GevurahBasicAttack();
            ItemManager itemManager = new ItemManager();

            string playerName = "rektorOfSouls";
            PlayerNameComponent playerNameComponent = new PlayerNameComponent(playerName);
            ActivePlayerComponent activePlayerComponent = new ActivePlayerComponent();
            playerBuilder.WithComponents(PlayerSpace, physics_, 
                    playerProperties_, healthBar_, inputListener, facingComponent, teamComponent, levelComponent, basicAttackListener, gevurahBasicAttack, itemManager, playerNameComponent, activePlayerComponent);
            var playerObject = playerBuilder.Build();
            var shockwaveSkill =
                Skill.Builder()
                    .WithCooldown(TimeSpan.FromSeconds(1))
                    .WithSkillFunction(Gevurah.Shockwave)
                    .WithCommandment(Commandment.Ability1)
                    .Build();
            var arrowRainSkill =
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

            SkillComponent playerSkillComponent = new SkillComponent(shockwaveSkill, arrowRainSkill, chargeShot, archerRoll);
            playerObject.AttachComponent(playerSkillComponent);

            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(playerObject, "Player");
            objects.Add(playerObject);
            return objects;
        }
    }
}