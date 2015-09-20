using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class CollisionDestructibleComponent : Component
    {
        public CollisionDestructibleComponent(DxGame game)
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<CollisionMessage>(HandleCollision);
        }

        protected void HandleCollision(CollisionMessage collisionMessage)
        {
            if (collisionMessage.CollisionDirections.Any())
            {
                DxGame.RemoveGameObject(Parent);
            }
        }
    }
}