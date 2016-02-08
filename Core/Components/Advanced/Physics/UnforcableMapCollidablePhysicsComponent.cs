using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Physics
{
    /**
        <summary>
            A MapCollidablePhysicsComponent that does not interact with forces. Things like skills or fixed-path projectiles should use this
        </summary>
    */

    [DataContract]
    [Serializable]
    [ProtoContract]
    public class UnforcableMapCollidablePhysicsComponent : MapCollidablePhysicsComponent
    {
        protected UnforcableMapCollidablePhysicsComponent(DxVector2 velocity, DxVector2 acceleration,
            SpatialComponent space, UpdatePriority updatePriority) : base(velocity, acceleration, space, updatePriority) {}

        public override void AttachForce(Force force)
        {
            // Do nothing
        }

        public new static UnforcableMapCollidablePhysicsComponentBuilder Builder()
        {
            return new UnforcableMapCollidablePhysicsComponentBuilder();
        }

        public class UnforcableMapCollidablePhysicsComponentBuilder : MapCollidablePhysicsComponentBuilder
        {
            public override PhysicsComponent Build()
            {
                var physics = new UnforcableMapCollidablePhysicsComponent(velocity_, acceleration_, space_,
                    updatePriority_);
                foreach(Force force in forces_)
                {
                    physics.forces_.Add(force);
                }
                return physics;
            }
        }
    }
}