using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;

namespace DXGame.Core.Components.Advanced
{
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

        public override void SerializeTo(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void DeserializeFrom(NetIncomingMessage messsage)
        {
            throw new System.NotImplementedException();
        }
    }
}