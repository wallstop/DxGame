using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class CollisionDestructibleComponent : Component
    {
        public CollisionDestructibleComponent()
        {
            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleCollision);
        }

        protected void HandleCollision(CollisionMessage collisionMessage)
        {
            if(collisionMessage.CollisionDirections.Any())
            {
                Parent.Remove();
            }
        }
    }
}