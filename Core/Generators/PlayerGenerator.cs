using System.Collections.Generic;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Wrappers;
using DXGame.Main;
using DXGame.TowerGame.Behaviors;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator : Generator<GameObject>
    {
        // TODO: Make player sprites scalable
        private const string PLAYER_NONE = "PlayerNone";
        private const string PLAYER_WALKING_RIGHT = "PlayerWalkingRight";
        private const string PLAYER_WALKING_LEFT = "PlayerWalkingLeft";
        private const string PLAYER_JUMPING = "PlayerJumping";
        private const string PLAYER_2 = "Player2";
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
                    .WithDimensions(new DxVector2(50, 100)) // TODO: un-hard code these
                    .WithPosition(playerPosition);
            physics_ =
                new MapCollideablePhysicsComponent(game).WithMaxVelocity(MAX_VELOCITY)
                    .WithPositionalComponent(PlayerSpace);

            playerProperties_ = PlayerPropertiesComponent.DefaultPlayerProperties;
            playerProperties_.Health.CurrentValue -= 5;
            // TODO: Need to add state machine in (how?)

            // TODO Make sure animation component works 
            weapon_ = new RangedWeaponComponent(game).WithPhysicsComponent(physics_).WithDamage(50);

            // TODO
            healthBar_ = new FloatingHealthIndicator(game, new DxVector2(-10, -10), Color.Green,
                Color.Aquamarine, playerProperties_, PlayerSpace);
            healthBar_.LoadContent();
            game_ = game;
        }

        public override List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var playerBuilder = GameObject.Builder();
            playerBuilder.WithComponents(PlayerSpace, physics_, weapon_,
                playerProperties_, healthBar_);
            var player = playerBuilder.Build();
            var animationBuilder = AnimationComponent.Builder().WithDxGame(game_).WithPosition(PlayerSpace);
            var playerStateMachine = PlayerBehaviorFactory.GevurahBehavior(game_, animationBuilder,
                Player.PlayerFrom(player, "Gevurah"));
            animationBuilder.WithStateMachine(playerStateMachine);
            player.AttachComponent(playerStateMachine);
            player.AttachComponent(animationBuilder.Build());
            objects.Add(player);
            return objects;
        }
    }
}