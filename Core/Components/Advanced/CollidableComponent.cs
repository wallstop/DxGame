using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using ProtoBuf;

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
    [ProtoContract]
    public class CollidableComponent : Component
    {
        [ProtoMember(1)]
        [DataMember]
        public ReadOnlyCollection<CollidableDirection> CollidableDirections { get; }

        [IgnoreDataMember]
        public IEnumerable<DxLine> CollisionLines
        {
            get
            {
                var collisionLines = new List<DxLine>(CollidableDirections.Count);
                foreach(var collisionDirection in CollidableDirections)
                {
                    switch(collisionDirection)

                    {
                        case CollidableDirection.Down:
                            collisionLines.Add(Spatial.Space.BottomBorder);
                            break;
                        case CollidableDirection.Left:
                            collisionLines.Add(Spatial.Space.LeftBorder);
                            break;
                        case CollidableDirection.Right:
                            collisionLines.Add(Spatial.Space.RightBorder);
                            break;
                        case CollidableDirection.Up:
                            collisionLines.Add(Spatial.Space.TopBorder);
                            break;
                    }
                }
                return collisionLines;
            }
        }

        [DataMember]
        [ProtoMember(2)]
        public SpatialComponent Spatial { get; }

        protected CollidableComponent(IList<CollidableDirection> collidableDirections, SpatialComponent spatial)
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
            List<CollidableDirection> collidableDirections = CollisionDirectionsFrom(velocity);
            collidableDirections.RemoveAll(direction => !CollidableDirections.Contains(direction));
            return collidableDirections;
        }

        public static List<CollidableDirection> CollisionDirectionsFrom(DxVector2 velocity)
        {
            /* Should only be able to collide a max of two directions at once */
            int maxCollisionDirections = 2;
            List<CollidableDirection> collisions = new List<CollidableDirection>(maxCollisionDirections);
            if(velocity.X > 0)
            {
                collisions.Add(CollidableDirection.Left);
            }
            else if(velocity.X < 0)
            {
                collisions.Add(CollidableDirection.Right);
            }

            if(velocity.Y < 0)
            {
                collisions.Add(CollidableDirection.Down);
            }
            else if(velocity.Y > 0)
            {
                collisions.Add(CollidableDirection.Up);
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
                return new CollidableComponent(collidableDirections_, spatial_);
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