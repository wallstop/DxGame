using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public class CollisionModel : Model
    {
        public ISpatialTree<PhysicsComponent> Collidables { get; private set; }

        public CollisionModel()
        {
            MessageHandler.RegisterMessageHandler<PhysicsMessage>(HandlePhysicsMessage);
        }

        public override bool ShouldSerialize => false;

        protected override void Update(DxGameTime gameTime)
        {
            List<PhysicsComponent> physics = DxGame.Instance.DxGameElements.OfType<PhysicsComponent>().ToList();
            Collidables = new RTree<PhysicsComponent>(physicsComponent => physicsComponent.Space, physics);
        }

        private void HandlePhysicsMessage(PhysicsMessage message)
        {
            HashSet<PhysicsComponent> affectedPhysicsComponents = new HashSet<PhysicsComponent>();
            /* We may have shapes that overlap - collect them into a set so we don't double-send messages */
            foreach (IShape area in message.AffectedAreas)
            {
                foreach (PhysicsComponent physicsComponent in Collidables.InRange(area))
                {
                    affectedPhysicsComponents.Add(physicsComponent);
                }
            }
            foreach (PhysicsComponent physicsComponent in affectedPhysicsComponents)
            {
                message.Interaction(message.Source, physicsComponent);
            }
        }
    }
}