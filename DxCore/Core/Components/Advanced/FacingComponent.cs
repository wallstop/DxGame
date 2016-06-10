using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Utils.Distance;

namespace DxCore.Core.Components.Advanced
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
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
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