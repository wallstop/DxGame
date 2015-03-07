using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced
{
    public class PlayerStateComponent : StateComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapCollideablePhysicsComponent));
        protected SimplePlayerInputComponent input_;

        public PlayerStateComponent(DxGame game)
            : base(game)
        {
            RegisterMessageHandler(typeof (CollisionMessage), HandleCollision);
            UpdatePriority = UpdatePriority.STATE;
        }

        public PlayerStateComponent WithInput(SimplePlayerInputComponent input)
        {
            Debug.Assert(input != null, "Player input component cannot be null on assignment");
            input_ = input;
            return this;
        }

        public void HandleCollision(Message message)
        {
            var collisionMessage = GenericUtils.CheckedCast<CollisionMessage>(message);
            var collisionDirections = collisionMessage.CollisionDirections;

            switch (input_.StateRequest)
            {
            case "Walking_Left":
                State = !collisionDirections.Contains(CollisionDirection.West) ? "Walking_Left" : "None";
                break;
            case "Walking_Right":
                State = !collisionDirections.Contains(CollisionDirection.East) ? "Walking_Right" : "None";
                break;
            case "Jumping":
                if (collisionDirections.Contains(CollisionDirection.South))
                {
                    State = "Jumping";
                }
                break;
            case "None":
                State = "None";
                break;
            }
        }
    }
}