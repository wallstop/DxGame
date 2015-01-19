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
        }

        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
            var messageType = message.GetType();

            if (messageType == typeof(CollisionMessage))
            {
                DxGame.RemoveGameObject(Parent);
            }
        }
    }
}