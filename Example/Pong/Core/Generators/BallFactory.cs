using DxCore.Core;
using DxCore.Core.Components.Advanced.Position;
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
            MapBoundedSpatialComponent ballSpatial = new MapBoundedSpatialComponent(position - radialVector, radialVector * 2);
            CollisionBouncablePhyicsComponent ballPhysics = new CollisionBouncablePhyicsComponent(ballSpatial);
            DrawableCircle ballDrawable = new DrawableCircle(ballSpatial, Color.Black, radius);

            GameObject ball = GameObject.Builder().WithComponents(ballSpatial, ballPhysics, ballDrawable).Build();
            return ball;
        }
    }
}
