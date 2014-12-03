﻿using System.Collections.Generic;
using DXGame.Core.Components.Advanced;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator : ObjectGenerator<GameObject>
    {
        private const string PLAYER = "Player";
        private const float MAX_VELOCITY = 5.0f;
        private readonly SimplePlayerInputComponent input_;
        private readonly PhysicsComponent physics_;
        private readonly PositionalComponent position_;
        private readonly SpatialComponent space_;
        private readonly SimpleSpriteComponent sprite_;

        public PlayerGenerator(Vector2 playerPosition, Rectangle bounds = new Rectangle())
        {
            position_ = new PositionalComponent().WithPosition(playerPosition).WithBounds(bounds);
            space_ =
                new BoundedSpatialComponent().WithXMin(bounds.X)
                    .WithXMax(bounds.Width)
                    .WithXMin(bounds.Y)
                    .WithYMax(bounds.Height)
                    .WithPosition(position_)
                    .WithWidthAndHeight(new Vector2(50, 100));  // TODO: un-hard code these
            physics_ = new PhysicsComponent().WithMaxVelocity(MAX_VELOCITY).WithSpatialComponent(space_);
            sprite_ = new SimpleSpriteComponent().WithAsset(PLAYER).WithPosition(position_);
            input_ = new SimplePlayerInputComponent().WithPhysics(physics_);
        }

        public override List<GameObject> Generate()
        {
            var objects = new List<GameObject>();
            var player = new GameObject();
            player.AttachComponents(position_, physics_, sprite_, input_);
            objects.Add(player);
            return objects;
        }
    }
}