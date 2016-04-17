using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Developer;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using ProtoBuf;

namespace DXGame.TowerGame.Skills.Gevurah
{
    public static class Gevurah
    {
        public static void Shockwave(GameObject parent, DxGameTime startTime, DxGameTime gameTime)
        {
            var position = parent.ComponentOfType<SpatialComponent>().Center;
            var physicsMessage = new PhysicsMessage();
            physicsMessage.AffectedAreas.Add(new DxCircle(position, 100));
            physicsMessage.Source = parent;
            // TODO: Hmmm this super sucks make better pls
            physicsMessage.Interaction =
                (gameObject, destination) =>
                    new ShockwaveClosure(gameTime.TotalGameTime).ShockwaveInteraction(gameObject, destination);
            physicsMessage.Emit();
        }

        public static void RainOfArrows(GameObject parent, DxGameTime startTime, DxGameTime gameTime)
        {
            var position = parent.ComponentOfType<SpatialComponent>();
            var facing = parent.ComponentOfType<FacingComponent>();
            var arrowRainLauncher = new ArrowRainLauncher(parent, position.Center, facing.Facing);
            var arrowRainObject = GameObject.Builder().WithComponent(arrowRainLauncher).Build();
            arrowRainObject.Create();
        }

        public static void ChargeShot(GameObject parent, DxGameTime startTime, DxGameTime endTime)
        {
            SpatialComponent position = parent.ComponentOfType<SpatialComponent>();
            FacingComponent facing = parent.ComponentOfType<FacingComponent>();
            DxVector2 force = new DxVector2(15, 0);
            if(facing.Facing == Direction.West)
            {
                force *= -1;
            }
            DxVector2 dimensions = new DxVector2(60, 20);
            DxVector2 startPoint = position.Center;
            switch(facing.Facing)
            {
                case Direction.West:
                    startPoint.X -= dimensions.X;
                    startPoint.Y = position.Center.Y;
                    break;
                case Direction.East:
                    startPoint.X += position.Width;
                    break;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine starting point for charge shot while facing {facing.Facing}");
            }

            SpatialComponent spatial =
                (SpatialComponent)
                    SpatialComponent.Builder().WithDimensions(dimensions).WithPosition(startPoint).Build();

            PhysicsComponent physics =
                UnforcablePhysicsComponent.Builder().WithVelocity(force).WithSpatialComponent(spatial).Build();

            ChargeShotComponent chargeShot = new ChargeShotComponent(spatial, facing.Facing,
                endTime.TotalGameTime - startTime.TotalGameTime);

            GameObject chargeShotObject = GameObject.Builder().WithComponents(spatial, physics, chargeShot).Build();
            chargeShotObject.Create();
        }

        public static void BearTrapRoll(GameObject parent, DxGameTime startTime, DxGameTime endTime)
        {
            FacingComponent facing = parent.ComponentOfType<FacingComponent>();
            DxVector2 initialVelocity = new DxVector2(13, 0);
            if(facing.Facing == Direction.West)
            {
                initialVelocity.X *= -1;
            }

            Force rollForce = new Force(initialVelocity, DxVector2.EmptyVector, Force.InstantDisipation, "Archer Roll");
            PhysicsComponent physics = parent.ComponentOfType<PhysicsComponent>();
            physics.AttachForce(rollForce);

            DxVector2 bearTrapDimensions = new DxVector2(50, 20);
            PositionalComponent entityPosition = parent.ComponentOfType<PositionalComponent>();
            DxVector2 startPoint = entityPosition.Position;
            SpatialComponent bearTrapSpatial =
                (SpatialComponent)
                    SpatialComponent.Builder().WithDimensions(bearTrapDimensions).WithPosition(startPoint).Build();
            PhysicsComponent bearTrapPhysics =
                UnforcableMapCollidablePhysicsComponent.Builder()
                    .WithGravity()
                    .WithSpatialComponent(bearTrapSpatial)
                    .WithVelocity(new DxVector2(0, 7))
                    .Build();
            BearTrap bearTrap = new BearTrap(bearTrapSpatial);
            GameObject bearTrapObject =
                GameObject.Builder().WithComponents(bearTrapSpatial, bearTrapPhysics, bearTrap).Build();
            bearTrapObject.Create();
        }

        private static Tuple<bool, double> ShockwaveDamage(GameObject source, GameObject destination)
        {
            if(source == destination)
            {
                return Tuple.Create(false, 0.0);
            }
            return Tuple.Create(true, 3.0);
        }

        [Serializable]
        [DataContract]
        [ProtoContract]
        private class ShockwaveClosure
        {
            private static readonly TimeSpan DURATION = TimeSpan.FromSeconds(0.75);

            [DataMember] [ProtoMember(1)] private DxVector2 initialAcceleration_;

            [DataMember]
            [ProtoMember(2)]
            private TimeSpan InitialTime { get; }

            public ShockwaveClosure(TimeSpan initial)
            {
                InitialTime = initial;
            }

            public void ShockwaveInteraction(GameObject source, PhysicsComponent destination)
            {
                var sourcePhysics = source.ComponentOfType<PhysicsComponent>();
                var difference = new DxVector2(destination.Space.Center) - new DxVector2(sourcePhysics.Space.Center);

                /* If there is no difference in physics' positions (exact), we can't enact force on it :( This also prevents us from interacting with ourself */
                if(difference.X == 0 && difference.Y == 0)
                {
                    return;
                }
                var minForce = 35;
                var maxForce = 37;

                var radians = difference.Radian;
                var targetRadian =
                    new DxRadian(ThreadLocalRandom.Current.NextFloat(radians.Value - (float) (Math.PI / 4),
                        radians.Value + (float) (Math.PI / 4)));
                var forceScalar = ThreadLocalRandom.Current.NextFloat(minForce, maxForce);
                var targetVelocityVector = targetRadian.UnitVector * forceScalar;
                initialAcceleration_ = targetVelocityVector;

                /* Apply force... */
                var force = new Force(targetVelocityVector, targetVelocityVector, ShockwaveDissipation, "Shockwave");
                destination.AttachForce(force);

                /* ...and then damage (if we can) */
                DamageMessage damageDealt = new DamageMessage {Source = source, DamageCheck = ShockwaveDamage, Target = destination.Parent?.Id};
                damageDealt.Emit();

                /* ... and attach a life sucker (just to be evil) */
                if(!ReferenceEquals(destination.Parent, null))
                {
                    LifeSuckerComponent lifeSucker = new LifeSuckerComponent();
                    destination.Parent.AttachComponent(lifeSucker);
                    EntityCreatedMessage entityCreated = new EntityCreatedMessage(lifeSucker);
                    entityCreated.Emit();
                }
            }

            private Tuple<bool, DxVector2> ShockwaveDissipation(DxVector2 externalVelocity,
                DxVector2 currentAcceleration, DxGameTime gameTime)
            {
                TimeSpan totalElapsed = gameTime.TotalGameTime - InitialTime;
                return Tuple.Create(DURATION < totalElapsed, initialAcceleration_ * 0.01);
            }
        }
    }
}