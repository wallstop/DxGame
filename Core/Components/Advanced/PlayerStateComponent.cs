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
        protected string stateRequest_;

        public PlayerStateComponent(DxGame game)
            : base(game)
        {
            RegisterMessageHandler(typeof (CollisionMessage), HandleCollision);
            RegisterMessageHandler(typeof (StateChangeRequestMessage), HandleStateChangeRequest);
            UpdatePriority = UpdatePriority.STATE;
        }

        public void HandleStateChangeRequest(Message message)
        {
            var stateRequest = GenericUtils.CheckedCast<StateChangeRequestMessage>(message);
            stateRequest_ = stateRequest.State;
        }

        public void HandleCollision(Message message)
        {
            var collisionMessage = GenericUtils.CheckedCast<CollisionMessage>(message);
            var collisionDirections = collisionMessage.CollisionDirections;

            // If we don't know about a request, don't do anything :( 
            if (stateRequest_ == null)
            {
                return;
            }

            switch (stateRequest_)
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

            stateRequest_ = null;
        }
    }
}