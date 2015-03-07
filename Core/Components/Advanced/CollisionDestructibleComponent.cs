using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
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
            DxGame.RemoveGameObject(Parent);
        }
    }
}