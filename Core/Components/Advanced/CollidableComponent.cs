using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    public enum CollidableDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    [Serializable]
    [DataContract]
    public class CollidableComponent : SpatialComponent
    {
        [DataMember]
        public List<CollidableDirection> CollidableDirections { get; private set; } = new List<CollidableDirection>();

        public CollidableComponent(DxGame game)
            : base(game)
        {
        }

        /* TODO: Builder pattern */
        public CollidableComponent WithCollidableDirections(IEnumerable<CollidableDirection> directions)
        {
            var collidableDirections = directions as IList<CollidableDirection> ?? directions.ToList();
            Validate.IsNotEmpty(collidableDirections,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(directions)));
            CollidableDirections.Clear();
            CollidableDirections.AddRange(collidableDirections);
            return this;
        }

        /*
            Given some kind of position
        */
        public bool CollidesWith(DxVector2 velocity)
        {
            return (velocity.X > 0 && CollidableDirections.Contains(CollidableDirection.Left)) ||
                   (velocity.X < 0 && CollidableDirections.Contains(CollidableDirection.Right)) ||
                   (velocity.Y < 0 && CollidableDirections.Contains(CollidableDirection.Down)) ||
                   (velocity.Y > 0 && CollidableDirections.Contains(CollidableDirection.Up));
        }

        public List<CollidableDirection> CollisionDirections(DxVector2 velocity)                                                                                       
        {
            /* Should only be able to collide a max of two directions at once */
            int maxCollisionDirections = 2;
            List<CollidableDirection> collisions = new List<CollidableDirection>(maxCollisionDirections);
            if (velocity.X > 0 && CollidableDirections.Contains(CollidableDirection.Left))
            {
                collisions.Add(CollidableDirection.Left);
            } else if (velocity.X < 0 && CollidableDirections.Contains(CollidableDirection.Right))
            {
                collisions.Add(CollidableDirection.Right);
            }

            if (velocity.Y < 0 && CollidableDirections.Contains(CollidableDirection.Up))
            {
                collisions.Add(CollidableDirection.Up);
            } else if (velocity.Y > 0 && CollidableDirections.Contains(CollidableDirection.Down))
            {
                collisions.Add(CollidableDirection.Down);
            }
            return collisions;
        }
    }
}