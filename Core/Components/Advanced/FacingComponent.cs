﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;
using DXGame.Main;

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
        public Direction Facing { get; private set; }

        public FacingComponent(DxGame game)
            : base(game)
        {
            MessageHandler.RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
        }

        private void HandleCommandMessage(CommandMessage command)
        {
            switch (command.Commandment)
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