using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using NLog;

namespace DxCore.Core.Physics
{
    public static class SensorFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void WorldCollisionSensor(Body body, Fixture fixture, PhysicsComponent initializedComponent)
        {
            // TODO: Expand to all directions

            /* 
                Note: This sensor will emit a message for *EACH* map tile that it collides with, per frame. 
                Might want to fix that
            */
            AABB fixtureBounds;
            fixture.GetAABB(out fixtureBounds, 0);

            Vector2 lower = fixtureBounds.LowerBound - body.Position;
            Vector2 upper = fixtureBounds.UpperBound - body.Position;

            Fixture mapCollisionSensor = FixtureFactory.AttachEdge(new Vector2(lower.X + 0.1f, upper.Y),
                new Vector2(upper.X - 0.1f, upper.Y), body, null);
            mapCollisionSensor.IsSensor = true;

            mapCollisionSensor.OnCollision += (self, maybeMapTile, contact) =>
            {
                IWorldCollidable worldCollidable = maybeMapTile.UserData as IWorldCollidable;
                if(ReferenceEquals(worldCollidable, null))
                {
                    return false;
                }
                CollisionMessage worldCollision = new CollisionMessage();
                worldCollision.WithDirectionAndSource(Direction.South, worldCollidable);
                worldCollision.Target = initializedComponent.Parent.Id;
                Logger.Info($"Triggered map collision: {contact}");
                worldCollision.Emit();
                return false;
            };
        }
    }
}
