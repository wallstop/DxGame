using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class CollisionDestructibleComponent : Component
    {
        public CollisionDestructibleComponent(DxGame game)
            : base(game)
        {
            RegisterMessageHandler(typeof (CollisionMessage), HandleCollision);
        }

        protected void HandleCollision(Message message)
        {
            var collisionMessage = GenericUtils.CheckedCast<CollisionMessage>(message);
            if (collisionMessage.CollisionDirections.Any())
            {
                DxGame.RemoveGameObject(Parent);
            }
        }
    }
}