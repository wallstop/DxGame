﻿using System.Collections.Generic;
using DXGame.Core.Components;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Generators
{
    public class PlayerGenerator : ObjectGenerator<GameObject>
    {
        private const string PLAYER = "Player";
        private const float MAX_VELOCITY = 1.0f;
        private readonly SimplePlayerInputComponent input_;
        private readonly PhysicsComponent physics_;
        private readonly PositionalComponent position_;
        private readonly SimpleSpriteComponent sprite_;

        public PlayerGenerator(Vector2 playerPosition)
        {
            position_ = new PositionalComponent().WithPosition(playerPosition);
            physics_ = new PhysicsComponent().WithMaxVelocity(MAX_VELOCITY).WithPosition(position_);
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