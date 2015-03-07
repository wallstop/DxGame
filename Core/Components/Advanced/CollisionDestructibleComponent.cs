using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;

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
    }
}