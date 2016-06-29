using System;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using Pong.Core.Components;

namespace Pong.Core.Generators
{
    // That's a paddlin
    public static class PaddleFactory
    {
        public static GameObject PlayerPaddle(DxVector2 position)
        {
            PlayerInputListener playerinputListener = new PlayerInputListener();

            GameObject paddle = Paddle(position, "Paddle/Player");
            paddle.AttachComponent(playerinputListener);
            return paddle;
        }

        public static GameObject EnemyPaddle(DxVector2 position)
        {
            RandomCommander randomCommander = new RandomCommander(TimeSpan.FromSeconds(1/4.0), new []{Commandment.MoveDown, Commandment.MoveUp});
            GameObject paddle = Paddle(position, "Paddle/Enemy");
            paddle.AttachComponent(randomCommander);
            return paddle;
        }

        private static GameObject Paddle(DxVector2 position, string assetPath)
        {
            SpatialComponent paddleSpatial = new MapBoundedSpatialComponent(position, new DxVector2(50, 175));
            PhysicsCollisionComponent paddleCollision = new PhysicsCollisionComponent(paddleSpatial);
            PaddleCommandProcessor paddleCommandProcessor = new PaddleCommandProcessor();
            PhysicsComponent paddlePhysics =
                PhysicsComponent.Builder().WithSpatialComponent(paddleSpatial).WithAirResistance().Build();
            SimpleSpriteComponent paddleDrawable =
                SimpleSpriteComponent.Builder()
                    .WithAsset(assetPath)
                    .WithPosition(paddleSpatial)
                    .WithBoundingBox(paddleSpatial.Space)
                    .Build();

            GameObject paddle = GameObject.Builder()
                .WithComponents(paddleDrawable, paddleCollision, paddleSpatial, paddlePhysics, paddleCommandProcessor)
                .Build();

            return paddle;
        }

    }
}
