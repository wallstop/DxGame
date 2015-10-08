using System;
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

        public static void RainOfArrows(GameObject parent, DxGameTime gameTime)
        {
            var facingComponent = parent.ComponentOfType<FacingComponent>();
            var position = parent.ComponentOfType<SpatialComponent>();
            
            /* 
                Constants for the "arrow rain" ability. The general gist is you shoot an arrow 
                (in the way that you're facing) and arrows rain down until they hit the ground
                OR they have traveled some distance
            */
            var arrowRainGap = 300;
            var arrowRainHeight = 100;
            var arrowRainWidth = 300;
            var arrowRainDepth = 300;

            var target = position.Center;
            target.X += ((facingComponent.Facing == Direction.West ? -1 : 1) * arrowRainGap);
            target.Y -= arrowRainHeight;

            /* Sort map tiles, tallest first (lowest y coordinate) */
            var mapTilesInRange =
                DxGame.Instance.Model<MapModel>()
                    .Map.Collidables.InRange(new DxRectangle(target.X, target.Y, arrowRainWidth, arrowRainDepth))
                    .Select(tile => tile.Spatial.Space).ToList();
            mapTilesInRange.Sort((tile1, tile2) => (int)(tile2.Y - tile1.Y));

            /* 
                We could do some smart math, but for now just raycast downwards until we either 
                reach (depth) or (map tile) because it's easy enough 
            */
            var physicsMessage = new PhysicsMessage();
            var maxDepth = (int)target.Y + arrowRainDepth;
            var rectangleBegin = (int)target.X;
            var lastDepth = maxDepth;
            for (int i = (int)target.X; i < (int)target.X +  arrowRainWidth; ++i)
            {
                var index = i;
                Func<DxRectangle, bool> intersections = tile => tile.X <= index && index < tile.X + tile.Width;
                int depth = maxDepth;
                if (mapTilesInRange.Any(intersections))
                {
                    depth = (int)mapTilesInRange.First(intersections).Y;
                }
                /* Are we at a different depth & have moved at least one pixel? */
                if (depth != lastDepth && rectangleBegin != i)
                {
                    /* If so, great, that means we're at a new rectangular bound, so cap the old one off & ship it */
                    physicsMessage.AffectedAreas.Add(new DxRectangle(rectangleBegin, target.Y, (i - rectangleBegin), (depth - target.Y)));
                    rectangleBegin = i;
                    lastDepth = depth;
                }
            }
            physicsMessage.AffectedAreas.Add(new DxRectangle(rectangleBegin, target.Y, (arrowRainWidth - rectangleBegin), (lastDepth - target.Y)));

            physicsMessage.Source = parent;
            physicsMessage.Interaction = RainOfArrowsInteraction;
            DxGame.Instance.BroadcastMessage(physicsMessage);
        }

        private static void RainOfArrowsInteraction(GameObject source, PhysicsComponent destination)
        {


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
            var force = new Force(targetVelocityVector, new DxVector2(), ShockwaveDissipation, "Shockwave");
            destination.AttachForce(force);

            /* ...and then damage (if we can) */
            var damageDealt = new DamageMessage {Source = source, DamageCheck = ShockwaveDamage};
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
            return Tuple.Create(true, new DxVector2());
        }
    }
}