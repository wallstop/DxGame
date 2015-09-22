using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Enemy;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Player;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Skills;
using DXGame.Main;
using DXGame.TowerGame.Actions;
using DXGame.TowerGame.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator
    {
        // TODO: Make player sprites scalable
        private static readonly DxVector2 MAX_VELOCITY = new DxVector2(5.0f, 20.0f);
        private readonly DxGame game_;
        private readonly FloatingHealthIndicator healthBar_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;
        private readonly WeaponComponent weapon_;
        public SpatialComponent PlayerSpace { get; }

        public PlayerGenerator(DxGame game, DxVector2 playerPosition, DxRectangle bounds)
        {
            PlayerSpace =
                (BoundedSpatialComponent) new BoundedSpatialComponent(game).WithXMin(bounds.X)
                    .WithXMax(bounds.Width)
                    .WithXMin(bounds.Y)
                    .WithYMax(bounds.Height)
                    .WithDimensions(new DxVector2(50, 50)) // TODO: un-hard code these
                    .WithPosition(playerPosition);
            physics_ = MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(PlayerSpace).Build();

            playerProperties_ = PlayerPropertiesComponent.DefaultPlayerProperties;
            /* Fuck with the health so we can check if the hp bar works */
            playerProperties_.Health.CurrentValue -= 3;
            // TODO: Need to add state machine in (how?)

            // TODO Make sure animation component works 
            weapon_ = new RangedWeaponComponent(game).WithPhysicsComponent(physics_).WithDamage(50);

            // TODO make these colors not shit and/or blend into the current biome
            healthBar_ = new FloatingHealthIndicator(game, new DxVector2(-10, -10), Color.Green,
                Color.Aquamarine, playerProperties_, PlayerSpace);
            healthBar_.LoadContent();
            game_ = game;
        }

        public List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var playerBuilder = GameObject.Builder();
            var inputListener = new PlayerInputListener(game_);
            playerBuilder.WithComponents(PlayerSpace, physics_, weapon_,
                playerProperties_, healthBar_, inputListener);
            var playerObject = playerBuilder.Build();
            var player = Player.PlayerFrom(playerObject, "Gevurah");
            var shockwaveSkill =
                Skill.Builder().WithCooldown(TimeSpan.FromSeconds(1)).WithSkillFunction(Gevurah.Shockwave).Build();
            SkillActivater shockwaveActivator = (game, component, remainingCooldown) =>
            {
                return game.Model<InputModel>().FinishedEvents.Any(finishedEvent => finishedEvent.Key == Keys.E);
            };
            var skillComponent = new SkillComponent(game_, shockwaveSkill, shockwaveActivator);
            playerObject.AttachComponent(skillComponent);

            var animationBuilder = AnimationComponent.Builder().WithDxGame(game_).WithPosition(PlayerSpace);
            var playerStateMachine = PlayerActionFactory.GevurahBehavior(game_, animationBuilder,
                Player.PlayerFrom(playerObject, "Gevurah"));
            animationBuilder.WithStateMachine(playerStateMachine);
            playerObject.AttachComponent(playerStateMachine);
            playerObject.AttachComponent(animationBuilder.Build());
            objects.Add(playerObject);

            var simpleSpawner = SpawnerFactory.SimpleBoxSpawner();
            var simpleSpawnerOwner = GameObject.Builder().WithComponent(simpleSpawner).Build();
            objects.Add(simpleSpawnerOwner);

            return objects;
        }
    }
}