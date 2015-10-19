﻿using System;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Developer;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.TowerGame.Skills.Gevurah
{
    public static class Gevurah
    {
        public static void Shockwave(GameObject parent, DxGameTime gameTime)
        {
            var position = parent.ComponentOfType<SpatialComponent>().Center;
            var physicsMessage = new PhysicsMessage();
            physicsMessage.AffectedAreas.Add(new DxCircle(position, 100));
            physicsMessage.Source = parent;
            physicsMessage.Interaction = ShockwaveInteraction;
            DxGame.Instance.BroadcastMessage(physicsMessage);
        }

        public static void RainOfArrows(GameObject parent, DxGameTime gameTime)
        {
            var position = parent.ComponentOfType<SpatialComponent>();
            var facing = parent.ComponentOfType<FacingComponent>();
            var arrowRainLauncher = new ArrowRainLauncher(parent, position.Center, facing.Facing);
            var arrowRainObject = GameObject.Builder().WithComponent(arrowRainLauncher).Build();
            DxGame.Instance.AddAndInitializeGameObject(arrowRainObject);
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
            var minForce = 25;
            var maxForce = 37;

            var radians = difference.Radian;
            var targetRadian =
                new DxRadian(ThreadLocalRandom.Current.NextFloat(radians.Value - (float) (Math.PI / 4),
                    radians.Value + (float) (Math.PI / 4)));
            var forceScalar = ThreadLocalRandom.Current.NextFloat(minForce, maxForce);
            var targetVelocityVector = targetRadian.UnitVector * forceScalar;

            /* Apply force... */
            var force = new Force(targetVelocityVector, new DxVector2(), targetVelocityVector, ShockwaveDissipation, "Shockwave");
            destination.AttachForce(force);

            /* ...and then damage (if we can) */
            var damageDealt = new DamageMessage {Source = source, DamageCheck = ShockwaveDamage};
            destination.Parent?.BroadcastMessage(damageDealt);

            /* ... and attach a life sucker (just to be evil) */
            if (!ReferenceEquals(destination.Parent, null))
            {
                var lifeSucker = new LifeSuckerComponent();
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

        private static Tuple<bool, DxVector2> ShockwaveDissipation(DxVector2 externalVelocity, DxVector2 currentAcceleration, DxGameTime gameTime)
        {
            return Tuple.Create(true, new DxVector2());
        }
    }
}