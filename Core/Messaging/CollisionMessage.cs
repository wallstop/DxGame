using System.Collections.Generic;

namespace DXGame.Core.Messaging
{
    public enum CollisionDirection
    {
        North,
        East,
        South,
        West
    }

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
    }
}