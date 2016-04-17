using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Components.Advanced
{
    /** 
        <summary>
            Simple Component that listens for CommandMessages & determines the "orientation" that the entity (is apparently) facing
        </summary>
    */

    [Serializable]
    [DataContract]
    public class FacingComponent : Component
    {
        [DataMember]
        public Direction Facing { get; private set; } = Direction.East;

        public FacingComponent()
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
        }

        private void HandleCommandMessage(CommandMessage command)
        {
            if(!Equals(command.Target, Parent.Id))
            {
                return;
            }

            switch(command.Commandment)
            {
                case Commandment.MoveLeft:
                    Facing = Direction.West;
                    break;
                case Commandment.MoveRight:
                    Facing = Direction.East;
                    break;
            }
        }
    }
}