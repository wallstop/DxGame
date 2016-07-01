using DxCore.Core;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework;
using Pong.Core.Components;

namespace Pong.Core.Generators
{
    // Makes some balls, probably
    public static class BallFactory
    {
        public static GameObject Ball(DxVector2 position)
        {
            const float radius = 15;

            DxVector2 radialVector = new DxVector2(radius, radius);

            /* If we want to be centered on the position, we need to be offset the provided position by our radius (in both directions) */
            PhysicsComponent physics =
                PhysicsComponent.Builder()
                    .WithPosition(position - radialVector)
                    .WithBounds(radialVector * 2)
                    .WithCollisionType(PhysicsType.Dynamic)
                    .WithRestitution(2000)
                    .WithoutFriction()
                    .WithoutGravity()
                    .Build();
            DrawableCircle ballDrawable = new DrawableCircle(physics, Color.Black, radius);

            GameObject ball = GameObject.Builder().WithComponents(physics, ballDrawable).Build();
            return ball;
        }
    }
}
