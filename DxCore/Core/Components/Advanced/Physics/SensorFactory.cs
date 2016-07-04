using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using NLog;

namespace DxCore.Core.Components.Advanced.Physics
{
    public static class SensorFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void MapCollisionSensor(Body body, Fixture fixture, PhysicsComponent initializedComponent)
        {
            AABB fixtureBounds;
            fixture.GetAABB(out fixtureBounds, 0);

            Vector2 lower = fixtureBounds.LowerBound - body.Position;
            Vector2 upper = fixtureBounds.UpperBound - body.Position;

            Fixture mapCollisionSensor = FixtureFactory.AttachEdge(new Vector2(lower.X + 1, upper.Y),
                new Vector2(upper.X - 1, upper.Y), body, null);
            mapCollisionSensor.IsSensor = true;

            mapCollisionSensor.BeforeCollision += (Fixture self, Fixture maybeMapTile) =>
            {
                MapCollidable mapTile = maybeMapTile.UserData as MapCollidable;
                if(ReferenceEquals(mapTile, null))
                {
                    return false;
                }
                CollisionMessage mapCollision = new CollisionMessage();
                mapCollision.WithDirectionAndSource(Direction.South, mapTile);
                mapCollision.Target = initializedComponent.Parent.Id;
                Logger.Debug("Triggered map collision");
                mapCollision.Emit();
                return false;
            };
        }
    }
}
