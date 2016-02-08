using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    /** 
        <summary>
            Simple Component that listens for CommandMessages & determines the "orientation" that the entity (is apparently) facing
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class FacingComponent : Component
    {
        [DataMember]
        [ProtoMember(1)]
        public Direction Facing { get; private set; } = Direction.East;

        public FacingComponent()
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
        }

        private void HandleCommandMessage(CommandMessage command)
        {
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