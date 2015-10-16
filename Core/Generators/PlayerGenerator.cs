using System;
using System.Collections.Generic;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Enemy;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Player;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Skills;
using DXGame.Core.State;
using DXGame.Main;
using DXGame.TowerGame.Skills.Gevurah;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator
    {
        private readonly DxGame game_;
        private readonly FloatingHealthIndicator healthBar_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;
        private readonly WeaponComponent weapon_;
        public SpatialComponent PlayerSpace { get; }

        public PlayerGenerator(DxGame game, DxVector2 playerPosition, DxRectangle bounds)
        {
            PlayerSpace = (BoundedSpatialComponent)
                BoundedSpatialComponent.Builder().WithBounds(bounds)
                    .WithDimensions(new DxVector2(50, 50))
                    .WithPosition(playerPosition)
                    .Build();
            physics_ =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(PlayerSpace).Build();

            playerProperties_ = PlayerPropertiesComponent.DefaultPlayerProperties;
            /* Fuck with the health so we can check if the hp bar works */
            playerProperties_.Health.CurrentValue -= 3;
            // TODO: Need to add state machine in (how?)

            // TODO Make sure animation component works 
            weapon_ = new RangedWeaponComponent(game).WithPhysicsComponent(physics_).WithDamage(50);

            // TODO make these colors not shit and/or blend into the current biome
            healthBar_ =
                FloatingHealthIndicator.Builder()
                    .WithForegroundColor(Color.Green)
                    .WithEntityProperties(playerProperties_)
                    .WithPosition(PlayerSpace)
                    .Build();
            healthBar_.LoadContent();
            game_ = game;
        }

        public List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var playerBuilder = GameObject.Builder();
            var inputListener = new PlayerInputListener(game_);
            var facingComponent = new FacingComponent(game_);
            var teamComponent = new TeamComponent(game_, Team.PlayerTeam);
            playerBuilder.WithComponents(PlayerSpace, physics_, weapon_,
                playerProperties_, healthBar_, inputListener, facingComponent, teamComponent);
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
            var playerSkillComponent = new SkillComponent(game_, shockwaveSkill, arrowRainSkill);
            playerObject.AttachComponent(playerSkillComponent);
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(playerObject, "Player");
            objects.Add(playerObject);

            var simpleSpawner = SpawnerFactory.SimpleBoxSpawner();
            var simpleSpawnerOwner = GameObject.Builder().WithComponent(simpleSpawner).Build();
            objects.Add(simpleSpawnerOwner);

            return objects;
        }
    }
}