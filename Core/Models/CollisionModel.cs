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
        public ICollisionTree<PhysicsComponent> Collidables { get; private set; }

        public CollisionModel(DxGame game) 
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<PhysicsMessage>(HandlePhysicsMessage);
        }

        public override bool ShouldSerialize => false;

        protected override void Update(DxGameTime gameTime)
        {
            var physics = DxGame.DxGameElements.OfType<PhysicsComponent>().ToList();
            Collidables = new RTree<PhysicsComponent>(physicsComponent => physicsComponent.Space, physics);
        }

        private void HandlePhysicsMessage(PhysicsMessage message)
        {
            var affectedPhysicsComponents = new HashSet<PhysicsComponent>();
            foreach (var area in message.AffectedAreas)
            {
                foreach (var physicsComponent in Collidables.InRange(area))
                {
                    affectedPhysicsComponents.Add(physicsComponent);
                }
            }
            foreach (var physicsComponent in affectedPhysicsComponents)
            {
                message.Interaction(message.Source, physicsComponent);
            }
        }
    }
}