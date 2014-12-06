using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>
        WorldGravityComponent is a somewhat Unique component, being a singleton. Any PhysicsComponent that wishes to be acted upon by 
        the WorldGravity need only add themselves to it. This is done by:

        <code>
            PhysicsComponent myPhysicsComponent = new PhysicsComponent();
            WorldGravityComponent.Get().WithPhysicsComponent(myPhysicsComponent);
        </code>

        WorldGravity should generally affect only moveable Entities, like enemies and the players. Map Entities should not be added.
    </summary>        
    */
    public class WorldGravityComponent : UpdateableComponent
    {
        private const float gravity_ = -1.0f;
        private static readonly WorldGravityComponent singleGravity_ = new WorldGravityComponent();
        private static readonly HashSet<PhysicsComponent> physics_ = new HashSet<PhysicsComponent>();

        public static float Gravity
        {
            get { return gravity_; }
        }

        private WorldGravityComponent()
        {
        }

        public static WorldGravityComponent Get()
        {
            return singleGravity_;
        }

        public static WorldGravityComponent WithPhysicsComponent(PhysicsComponent physics)
        {
            Debug.Assert(physics != null, "World Gravity Component cannot be assigned to a null physics component");
            physics_.Add(physics);
            return singleGravity_;
        }

        public override bool Update(GameTime gameTime)
        {
            /*
                For now, I'm going to leave it up to each specific physics component to deal
                with the constant deceleration.
            */
            foreach (PhysicsComponent component in physics_)
            {
                Vector2 acceleration = component.Acceleration;
                acceleration.Y += gravity_;
                component.Acceleration = acceleration;
            }
            return true;
        }
    }
}