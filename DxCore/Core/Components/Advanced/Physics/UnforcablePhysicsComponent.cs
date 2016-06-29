using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Physics
{
    /**
        <summary>
            Just like a PhysicsComponent, except that no forces can affect it (after the initial forces)
        </summary>
    */

    [DataContract]
    [Serializable]
    public class UnforcablePhysicsComponent : PhysicsComponent
    {
        public UnforcablePhysicsComponent(DxVector2 velocity, DxVector2 acceleration, SpatialComponent position,
            UpdatePriority updatePriority) : base(velocity, acceleration, position, updatePriority) {}

        protected override void AttachForce(Force force)
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
                var physics = new UnforcablePhysicsComponent(velocity_, acceleration_, space_, updatePriority_);
                foreach(Force force in forces_)
                {
                    physics.forces_.Add(force);
                }
                return physics;
            }
        }
    }
}
