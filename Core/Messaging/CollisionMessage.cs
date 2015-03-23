using System.Collections.Generic;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Messaging
{
    public enum CollisionDirection
    {
        North,
        East,
        South,
        West
    }

    /*
        Collision Messages may or may not contain any CollisionDirections. Users should not rely on the fact that 
        the emission of a Collision Message actually means a Collision occurs.
    */

    public class CollisionMessage : Message
    {
        public List<CollisionDirection> CollisionDirections { get; set; }

        public CollisionMessage()
        {
            CollisionDirections = new List<CollisionDirection>();
        }

        public CollisionMessage WithDirection(CollisionDirection direction)
        {
            CollisionDirections.Add(direction);
            return this;
        }

        public CollisionMessage(DxVector2 collisionVector)
            : this()
        {
            if (collisionVector.X > 0)
            {
                WithDirection(CollisionDirection.East);
            }
            if (collisionVector.X < 0)
            {
                WithDirection(CollisionDirection.West);
            }
            if (collisionVector.Y > 0)
            {
                WithDirection(CollisionDirection.South);
            }
            if (collisionVector.Y < 0)
            {
                WithDirection(CollisionDirection.North);
            }
        }
    }
}