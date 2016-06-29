using System;
using System.Collections.Generic;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Core.Utils.Validate;

namespace Pong.Core.Components
{
    /**
        <summary>
            Emits collision messages when it detects an overlap in space with another physics component
        </summary>
    */
    public class PhysicsCollisionComponent : Component
    {
        private SpatialComponent Spatial { get; }

        public PhysicsCollisionComponent(SpatialComponent spatial)
        {
            Validate.Hard.IsNotNullOrDefault(spatial, () => this.GetFormattedNullOrDefaultMessage(spatial));
            Spatial = spatial;
        }

        private void CollisionInteraction(GameObject source, PhysicsComponent destination)
        {
            /* Are we ... us? If so, please ignore */
            if(destination.Parent == Parent)
            {
                return;
            }

            /* Otherwise, we bumped! Gotta find out how to tell em. */
            DxRadian offset = new DxLine(Spatial.Space.Center, destination.Space.Center);
            float offsetValue = offset.Value;
            /* 
                We have to figure out how we bumped. We should be able to do this by snapping to the closest direction 
                Note: We could use the sweet collision method in CollisionMessage, but that will give us two collision 
                directions. We should only be bumping into things... in one direction. Probably.
            */

            Direction collisionDirection;
            if(Math.PI / 4 < offsetValue && offsetValue <= 3 * Math.PI / 4)
            {
                collisionDirection = Direction.North;
            }
            else if(3 * Math.PI / 4 < offsetValue && offsetValue <= 5 * Math.PI / 4)
            {
                collisionDirection = Direction.West;
            }
            else if(5 * Math.PI / 4 < offsetValue && offsetValue <= 7 * Math.PI / 4)
            {
                collisionDirection = Direction.South;
            }
            else
            {
                collisionDirection = Direction.East;
            }

            CollisionMessage collision = new CollisionMessage {Target = destination.Parent.Id};
            collision.WithDirectionAndSource(collisionDirection, Parent);
            collision.Emit();
        }

        protected override void Update(DxGameTime gameTime)
        {
            PhysicsMessage collisionCheckBroadcast = new PhysicsMessage
            {
                Source = Parent,
                AffectedAreas = new List<IShape> {Spatial.Space},
                Interaction = CollisionInteraction
            };

            collisionCheckBroadcast.Emit();

            base.Update(gameTime);
        }
    }
}
