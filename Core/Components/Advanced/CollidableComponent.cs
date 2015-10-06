using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
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
    public class CollidableComponent : Component
    {
        [DataMember]
        public ReadOnlyCollection<CollidableDirection> CollidableDirections { get; }

        [DataMember]
        public SpatialComponent Spatial { get; }

        protected CollidableComponent(DxGame game, IList<CollidableDirection> collidableDirections,
            SpatialComponent spatial)
            : base(game)
        {
            CollidableDirections = new ReadOnlyCollection<CollidableDirection>(collidableDirections);
            Spatial = spatial;
        }

        public static CollidableComponentBuilder Builder()
        {
            return new CollidableComponentBuilder();
        }

        public bool CollidesWith(DxVector2 velocity)
        {
            return (velocity.X > 0 && CollidableDirections.Contains(CollidableDirection.Left)) ||
                   (velocity.X < 0 && CollidableDirections.Contains(CollidableDirection.Right)) ||
                   (velocity.Y < 0 && CollidableDirections.Contains(CollidableDirection.Down)) ||
                   (velocity.Y > 0 && CollidableDirections.Contains(CollidableDirection.Up));
        }

        /**
            Given a velocity, determines the collision directions (if any) that this object would present
        */

        public List<CollidableDirection> CollisionDirections(DxVector2 velocity)
        {
            /* Should only be able to collide a max of two directions at once */
            int maxCollisionDirections = 2;
            List<CollidableDirection> collisions = new List<CollidableDirection>(maxCollisionDirections);
            if (velocity.X > 0 && CollidableDirections.Contains(CollidableDirection.Left))
            {
                collisions.Add(CollidableDirection.Left);
            }
            else if (velocity.X < 0 && CollidableDirections.Contains(CollidableDirection.Right))
            {
                collisions.Add(CollidableDirection.Right);
            }

            if (velocity.Y < 0 && CollidableDirections.Contains(CollidableDirection.Up))
            {
                collisions.Add(CollidableDirection.Up);
            }
            else if (velocity.Y > 0 && CollidableDirections.Contains(CollidableDirection.Down))
            {
                collisions.Add(CollidableDirection.Down);
            }
            return collisions;
        }

        public class CollidableComponentBuilder : IBuilder<CollidableComponent>
        {
            protected readonly List<CollidableDirection> collidableDirections_ = new List<CollidableDirection>();
            protected SpatialComponent spatial_;

            public virtual CollidableComponent Build()
            {
                Validate.IsNotNull(spatial_, StringUtils.GetFormattedNullOrDefaultMessage(this, spatial_));
                var game = DxGame.Instance;
                return new CollidableComponent(game, collidableDirections_, spatial_);
            }

            public CollidableComponentBuilder WithCollidableDirections(IEnumerable<CollidableDirection> directions)
            {
                var collidableDirections = directions as IList<CollidableDirection> ?? directions.ToList();
                Validate.IsNotNull(collidableDirections,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, collidableDirections));
                collidableDirections_.Clear();
                collidableDirections_.AddRange(collidableDirections.Distinct());
                return this;
            }

            public CollidableComponentBuilder WithSpatial(SpatialComponent spatial)
            {
                spatial_ = spatial;
                return this;
            }
        }
    }
}