using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced.Player
{
    [Serializable]
    [DataContract]
    public class PlayerStateComponent : StateComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapCollideablePhysicsComponent));
        [DataMember] protected string stateRequest_;

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
            // If we want to jump, we jump! No problem.
            if (stateRequest_ == "Jumping")
            {
                State = "Jumping";
            }
        }

        public void HandleCollision(Message message)
        {
            var collisionMessage = GenericUtils.CheckedCast<CollisionMessage>(message);
            var collisionDirections = collisionMessage.CollisionDirections;

            // If we collide southwards, we stop jumping if we were jumping
            // Also, we consider ourselves to be jumping until we're not (collision southwards)
            if (State == "Jumping")
            {
                if (collisionDirections.Contains(CollisionDirection.South))
                    State = "None";
                else
                    return;
            }

            State = DetermineStateFromRequest(State, stateRequest_, collisionDirections);
            stateRequest_ = null;
        }

        private static string DetermineStateFromRequest(string currentState, string request,
            IEnumerable<CollisionDirection> collisionDirections)
        {
            switch (request)
            {
            case "Walking_Left":
                return !collisionDirections.Contains(CollisionDirection.West) ? "Walking_Left" : "None";
            case "Walking_Right":
                return !collisionDirections.Contains(CollisionDirection.East) ? "Walking_Right" : "None";
            case "None":
                return "None";
            }
            return currentState;
        }
    }
}