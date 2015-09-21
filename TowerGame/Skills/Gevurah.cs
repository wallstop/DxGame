using System;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.TowerGame.Skills
{
    public static class Gevurah
    {
        private static Random RGEN = new Random();
        public static void Shockwave(GameObject parent, DxGameTime gameTime)
        {
            var position = parent.ComponentOfType<SpatialComponent>().Center;
            var physicsMessage = new PhysicsMessage();
            physicsMessage.AffectedAreas.Add(DxRectangle.FromRange(position, 100));
            physicsMessage.Source = parent;
            physicsMessage.Interaction = ShockwaveInteraction;
            DxGame.Instance.BroadcastMessage(physicsMessage);
        }

        private static void ShockwaveInteraction(GameObject source, PhysicsComponent destination)
        {
            var sourcePhysics = source.ComponentOfType<PhysicsComponent>();
            if (ReferenceEquals(sourcePhysics, destination))
            {
                /* Don't interact with yourself */
                return;
            }
            var difference = new DxVector2(destination.Space.Center) - new DxVector2(sourcePhysics.Space.Center);
            var totalForce = 20;
            var totalDifferenceMagnitude = Math.Abs(difference.X) + Math.Abs(difference.Y);

            /* We only care about exactly equal to 0 to avoid NaN issues (division by 0) */
            if (totalDifferenceMagnitude == 0)
            {
                return;
            }

            difference.X = difference.X / totalDifferenceMagnitude * totalForce;
            difference.Y = difference.Y / totalDifferenceMagnitude * totalForce;
            /* 
                TODO: Find 'similar' vectors to these. Treat the difference as the normal vector for a plane in R2 (a line)
                and find vectors that are in the "same direction" 

                How the fuck do I do that?
            */
            difference.X += RGEN.Next(-totalForce, totalForce);
            difference.Y += RGEN.Next(-totalForce, totalForce);

            var force = new Force(difference, new DxVector2(), ShockwaveDissipation, "Shockwave");
            destination.AttachForce(force);
        }

        private static Tuple<bool, DxVector2> ShockwaveDissipation(DxVector2 externalVelocity,
            DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            return Tuple.Create(true, new DxVector2());
        }
    }
}
