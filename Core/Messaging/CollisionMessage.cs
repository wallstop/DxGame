using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils.Distance;

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
        public Dictionary<Direction, IIdentifiable> CollisionDirections { get; set;  } = new Dictionary<Direction, IIdentifiable>();

        [DataMember]
        public DxVector2 DisplacementVector { get; set; }

        public CollisionMessage()
        {
        }

        public CollisionMessage WithDirectionAndSource(Direction direction, IIdentifiable source)
        {
            CollisionDirections[direction] = source;
            return this;
        }

        public CollisionMessage(DxVector2 collisionVector, IIdentifiable source)
        {
            if (collisionVector.X > 0)
            {
                WithDirectionAndSource(Direction.East, source);
            }
            else if (collisionVector.X < 0)
            {
                WithDirectionAndSource(Direction.West, source);
            }

            if (collisionVector.Y > 0)
            {
                WithDirectionAndSource(Direction.South, source);
            }
            else if (collisionVector.Y < 0)
            {
                WithDirectionAndSource(Direction.North, source);
            }

            DisplacementVector = DxVector2.EmptyVector - collisionVector;
        }
    }
}