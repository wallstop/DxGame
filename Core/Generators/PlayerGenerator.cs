using System.Collections.Generic;
using DXGame.Core.Components.Advanced;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator : Generator<GameObject>
    {
        private const string PLAYER = "Player";
        private static readonly Vector2 MAX_VELOCITY = new Vector2(5.0f, 20.0f);
        private readonly SimplePlayerInputComponent input_;
        private readonly PhysicsComponent physics_;
        private readonly SpatialComponent space_;
        private readonly PlayerStateComponent state_;
        private readonly SimpleSpriteComponent sprite_;

        public PlayerGenerator(Vector2 playerPosition, Rectangle bounds = new Rectangle())
        {
            space_ =
                (BoundedSpatialComponent) new BoundedSpatialComponent().WithXMin(bounds.X)
                    .WithXMax(bounds.Width)
                    .WithXMin(bounds.Y)
                    .WithYMax(bounds.Height)
                    .WithDimensions(new Vector2(50, 100)) // TODO: un-hard code these
                    .WithPosition(playerPosition);
            physics_ = new MapCollideablePhysicsComponent().WithMaxVelocity(MAX_VELOCITY).WithPositionalComponent(space_);
            state_ = new PlayerStateComponent();
            sprite_ = new SimpleSpriteComponent().WithAsset(PLAYER).WithPosition(space_);
            input_ = new SimplePlayerInputComponent().WithPhysics(physics_).WithPlayerState(state_);
        }

        public override List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var player = new GameObject();
            WorldGravityComponent.WithPhysicsComponent(physics_);
            player.AttachComponents(space_, physics_, sprite_, input_, state_);
            objects.Add(player);
            return objects;
        }

        public SpatialComponent PlayerSpace
        {
            get { return space_; }
        }
    }
}