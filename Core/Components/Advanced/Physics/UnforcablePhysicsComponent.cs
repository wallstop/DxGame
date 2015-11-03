using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;

namespace DXGame.Core.Components.Advanced.Physics
{
    /**
        <summary>
            Just like a PhysicsComponent, except that no forces can affect it (after the initial forces)
        </summary>
    */
    [Serializable]
    [DataContract]
    public class UnforcablePhysicsComponent : PhysicsComponent
    {
        public UnforcablePhysicsComponent(DxVector2 velocity, DxVector2 acceleration, SpatialComponent position, UpdatePriority updatePriority) 
            : base(velocity, acceleration, position, updatePriority) {}

        public override void AttachForce(Force force)
        {
            // Do nothing
        }

        public new static UnforcablePhysicsComponentBuilder Builder()
        {
            return new UnforcablePhysicsComponentBuilder();
        }

        public class UnforcablePhysicsComponentBuilder : PhysicsComponentBuilder
        {
            public override PhysicsComponent Build()
            {
                var physics = new UnforcablePhysicsComponent(velocity_, acceleration_, space_,
                    updatePriority_);
                return physics;
            }
        }
    }
}
