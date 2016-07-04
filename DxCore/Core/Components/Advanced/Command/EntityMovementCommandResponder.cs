using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using NLog;

namespace DxCore.Core.Components.Advanced.Command
{
    /**
        <summary>
            Responds to Commandments by keeping track of relative forces to apply based on EntityProperties
        </summary>
    */

    [Serializable]
    [DataContract]
    public class EntityMovementCommandResponder : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnAttach()
        {
            RegisterMessageHandler<CommandMessage>(HandleCommandMessage);
            base.OnAttach();
        }

        private void HandleCommandMessage(CommandMessage command)
        {
            switch(command.Commandment)
            {
                case Commandment.MoveLeft:
                {
                    HandleMoveLeft();
                    break;
                }
                case Commandment.MoveRight:
                {
                    HandleMoveRight();
                    break;
                }
                case Commandment.MoveUp:
                {
                    HandleMoveUp();
                    break;
                }
                case Commandment.MoveDown:
                {
                    HandleMoveDown();
                    break;
                }
            }
        }

        private void HandleMoveLeft()
        {
            Logger.Debug($"{nameof(HandleMoveLeft)} called");
        }

        private void HandleMoveRight()
        {
            Logger.Debug($"{nameof(HandleMoveRight)} called");
        }

        private void HandleMoveUp()
        {
            Logger.Debug($"{nameof(HandleMoveUp)} called");
        }

        private void HandleMoveDown()
        {
            Logger.Debug($"{nameof(HandleMoveDown)} called");
        }
    }
}
