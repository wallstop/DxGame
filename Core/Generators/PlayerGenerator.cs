using System.Collections.Generic;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Player;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
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
        private readonly AnimationComponent animation_;
        private readonly FloatingHealthIndicator healthBar_;
        private readonly SimplePlayerInputComponent input_;
        private readonly PhysicsComponent physics_;
        private readonly EntityPropertiesComponent playerProperties_;
        private readonly PlayerStateComponent state_;
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
            state_ = new PlayerStateComponent(game);
            AddPlayerStates();
            animation_ = new AnimationComponent(game).WithPosition(PlayerSpace).WithState(state_);
            AddPlayerAnimations();
            weapon_ = new RangedWeaponComponent(game).WithPhysicsComponent(physics_).WithDamage(50);
            input_ =
                new SimplePlayerInputComponent(game).WithPhysics(physics_)
                    .WithPlayerState(state_)
                    .WithWeapon(weapon_)
                    .WithPlayerProperties(playerProperties_);
            // TODO
            healthBar_ = new FloatingHealthIndicator(game, new DxVector2(-10, -10), Color.Green,
                Color.Aquamarine, playerProperties_, PlayerSpace);
            healthBar_.LoadContent();
        }

        public override List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var player = new GameObject();
            player.WithComponents(PlayerSpace, physics_, animation_, input_, state_, weapon_,
                playerProperties_, healthBar_);
            objects.Add(player);
            return objects;
        }

        private void AddPlayerStates()
        {
            Validate.IsNotNullOrDefault(state_,
                $"StateComponent cannot be null/default during AddPlayerStates for {GetType()}");
            state_.AddStates("None", "Walking_Left", "Walking_Right", "Jumping");
        }

        private void AddPlayerAnimations()
        {
            Validate.IsNotNull(animation_, "AnimationComponent cannot be null during AddPlayerAnimations");
            animation_.AddAnimation("None", PLAYER_NONE);
            animation_.AddAnimation("Walking_Left", PLAYER_WALKING_LEFT);
            animation_.AddAnimation("Walking_Right", PLAYER_WALKING_RIGHT);
            animation_.AddAnimation("Jumping", PLAYER_JUMPING);
        }
    }
}