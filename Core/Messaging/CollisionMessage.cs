using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Messaging
{


    /*
        Collision Messages may or may not contain any CollisionDirections. Users should not rely on the fact that 
        the emission of a Collision Message actually means a Collision occurs.
    */

    [Serializable]
    [DataContract]
    public class CollisionMessage : Message
    {
        [DataMember]
        public List<Direction> CollisionDirections { get; set; }

        public CollisionMessage()
        {
            CollisionDirections = new List<Direction>();
        }

        public CollisionMessage WithDirection(Direction direction)
        {
            CollisionDirections.Add(direction);
            return this;
        }

        public CollisionMessage(DxVector2 collisionVector)
            : this()
        {
            if (collisionVector.X > 0)
            {
                WithDirection(Direction.East);
            }
            if (collisionVector.X < 0)
            {
                WithDirection(Direction.West);
            }
            if (collisionVector.Y > 0)
            {
                WithDirection(Direction.South);
            }
            if (collisionVector.Y < 0)
            {
                WithDirection(Direction.North);
            }
        }
    }
}