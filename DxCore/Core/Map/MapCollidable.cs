using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapCollidable : IIdentifiable
    {
        [DataMember]
        public UniqueId Id { get; private set; }

        [DataMember]
        public Tile Tile { get; private set; }

        [DataMember]
        public DxRectangle Space { get; private set; }

        [DataMember]
        public PhysicsComponent Physics { get; private set; }

        public MapCollidable(Tile tile, DxRectangle space)
        {
            Validate.Hard.IsNotNullOrDefault(tile);
            Validate.Hard.IsNotNullOrDefault(space);
            Id = new UniqueId();
            Tile = tile;
            Space = space;

            Physics =
                PhysicsComponent.Builder()
                    .WithPosition(space.TopLeft)
                    .WithBounds(space.Dimensions)
                    .WithCollisionGroup(CollisionGroup.Map)
                    .WithoutGravity()
                    .WithoutFriction()
                    .WithoutRestitution()
                    .WithPhysicsInitialization(SetPhysicsComponentStatic)
                    .Build();
        }

        private static void SetPhysicsComponentStatic(Body body, Fixture fixture, PhysicsComponent self)
        {
            body.BodyType = BodyType.Static;
        }
    }
}
