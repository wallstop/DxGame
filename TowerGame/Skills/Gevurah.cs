using System;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Damage;
using DXGame.Core.Components.Advanced.Particle;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Developer;
using DXGame.Core.Messaging;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

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
            var minForce = 2;
            var maxForce = 6;

            var radians = difference.Radian;
            var targetRadian =
                new DxRadian(ThreadLocalRandom.Current.NextFloat(radians.Value - (float) (Math.PI / 4),
                    radians.Value + (float) (Math.PI / 4)));
            var targetVelocityVector = targetRadian.UnitVector * ThreadLocalRandom.Current.NextFloat(minForce, maxForce);

            /* Apply force... */
            var force = new Force(targetVelocityVector, targetVelocityVector, ShockwaveDissipation, "Shockwave");
            destination.AttachForce(force);

            /* ...and then damage (if we can) */
            var damageDealt = new DamageMessage() {Source = source, DamageCheck = ShockwaveDamage};
            destination.Parent?.BroadcastMessage(damageDealt);

            /* ... and attach a life sucker (just to be evil) */
            var lifeSucker = new LifeSuckerComponent(DxGame.Instance);
            if (!ReferenceEquals(destination.Parent, null))
            {
                destination.Parent.AttachComponent(lifeSucker);
                DxGame.Instance.AddAndInitializeComponents(lifeSucker);
            }
        }

        private static Tuple<bool, double> ShockwaveDamage(GameObject source, GameObject destination)
        {
            if (source == destination)
            {
                return Tuple.Create(false, 0.0);
            }
            return Tuple.Create(true, 3.0);
        }

        private static Tuple<bool, DxVector2> ShockwaveDissipation(DxVector2 externalVelocity,
            DxVector2 externalAcceleration, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            var scaleFactor = gameTime.DetermineScaleFactor(DxGame.Instance);
            var negativeAcceleration = currentAcceleration * (float) scaleFactor / (float)DxGame.Instance.TargetFps / 4;
            var finalAcceleration = currentAcceleration - negativeAcceleration;
            if(Math.Abs(finalAcceleration.X) < 1 && Math.Abs(finalAcceleration.Y) < 1)
            {
                return Tuple.Create(true, new DxVector2());
            }
            return Tuple.Create(false, finalAcceleration);
        }
    }
}
