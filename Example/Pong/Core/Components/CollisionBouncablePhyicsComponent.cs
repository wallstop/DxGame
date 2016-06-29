using System;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;

namespace Pong.Core.Components
{
    /**
        <summary>
            Responds to collision events by doing a simple bounce, with a littttle bit of spin
        </summary>
    */
    public class CollisionBouncablePhyicsComponent : PhysicsComponent
    {
        public CollisionBouncablePhyicsComponent(SpatialComponent position)
            : base(DxVector2.EmptyVector, DxVector2.EmptyVector, position, UpdatePriority.PHYSICS) {}

        protected override void HandleCollisionMessage(CollisionMessage message)
        {
            DxVector2 velocity = Velocity;

            foreach(Direction collisionDirection in message.CollisionDirections.Keys)
            {
                switch(collisionDirection)
                {
                    case Direction.South:
                    case Direction.North:
                    {
                        velocity.Y *= -1;
                        break;
                    }
                    case Direction.West:
                    case Direction.East:
                    {
                        velocity.X *= -1;
                        break;
                    }
                }
            }
            
            DxRadian target = velocity.Radian;
            /*
                Puts some spin on her, where spin is loosely defined as PI / 10.

                This should have the affect of slightly altering the velocity that the 
                object is traveling at
            */
            const float piOverTen = (float) Math.PI / 10;
            float spin = ThreadLocalRandom.Current.NextFloat(-piOverTen, piOverTen);
            target.Value += spin;

            Velocity = target.UnitVector * velocity.Magnitude;
        }
    }
}
