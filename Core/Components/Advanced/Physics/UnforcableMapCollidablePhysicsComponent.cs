using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Physics
{
    /**
        A MapCollidablePhysicsComponent that does not interact with forces. Things like skills should use this
    */
    [Serializable]
    [DataContract]
    public class UnforcableMapCollidablePhysicsComponent : MapCollidablePhysicsComponent
    {
        protected UnforcableMapCollidablePhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration, SpatialComponent space, UpdatePriority updatePriority) 
            : base(game, velocity, acceleration, space, updatePriority)
        {
        }

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
                var physics = new UnforcableMapCollidablePhysicsComponent(game_, velocity_, acceleration_, space_, updatePriority_);
                return physics;
            }
        }
    }
}
