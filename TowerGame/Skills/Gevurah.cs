using System;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.TowerGame.Skills
{
    public static class Gevurah
    {
        //private static Random RGEN = new Random();
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
            var difference = new DxVector2(destination.Space.Center) - new DxVector2(sourcePhysics.Space.Center);

            /* If there is no difference in physics' positions (exact), we can't enact force on it :( This also prevents us from interacting with ourself */
            if (difference.X == 0 && difference.Y == 0)
            {
                return;
            }

            var minForce = 20;
            var maxForce = 37;

            var radians = difference.Radian;
            var targetRadian = new DxRadian(ThreadLocalRandom.Current.NextFloat(radians.Value - (float)(Math.PI / 4), radians.Value + (float)(Math.PI / 4)));
            var targetVelocityVector = targetRadian.UnitVector * ThreadLocalRandom.Current.Next(minForce, maxForce);

            var force = new Force(targetVelocityVector, new DxVector2(), ShockwaveDissipation, "Shockwave");
            destination.AttachForce(force);
        }

        private static Tuple<bool, DxVector2> ShockwaveDissipation(DxVector2 externalVelocity,
            DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            return Tuple.Create(true, new DxVector2());
        }
    }
}
