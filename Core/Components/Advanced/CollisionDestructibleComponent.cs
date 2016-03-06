using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class CollisionDestructibleComponent : Component
    {
        public CollisionDestructibleComponent(DxGame game)
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