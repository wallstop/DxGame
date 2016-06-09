﻿using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils.Distance;

namespace DxCore.Core.Models
{
    public class CollisionModel : Model
    {
        public ISpatialTree<PhysicsComponent> Collidables { get; private set; }

        public override void OnAttach()
        {
            RegisterMessageHandler<PhysicsMessage>(HandlePhysicsMessage);
        }

        public override bool ShouldSerialize => false;

        protected override void Update(DxGameTime gameTime)
        {
            List<PhysicsComponent> physics = DxGame.Instance.DxGameElements.OfType<PhysicsComponent>().ToList();
            Collidables = new RTree<PhysicsComponent>(physicsComponent => physicsComponent.Space, physics);
        }

        private void HandlePhysicsMessage(PhysicsMessage message)
        {
            var affectedPhysicsComponents = new HashSet<PhysicsComponent>();
            foreach(var area in message.AffectedAreas)
            {
                foreach(var physicsComponent in Collidables.InRange(area))
                {
                    affectedPhysicsComponents.Add(physicsComponent);
                }
            }
            foreach(var physicsComponent in affectedPhysicsComponents)
            {
                message.Interaction(message.Source, physicsComponent);
            }
        }
    }
}