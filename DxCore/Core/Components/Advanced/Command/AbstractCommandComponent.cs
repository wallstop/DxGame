using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;

namespace DxCore.Core.Components.Advanced.Command
{
    /**
        All components that issue "commandments" must derive from this type or else chaos will ensue.

        <summary>
            Base class for all Components that will issue Commandments
        </summary>
    */

    [Serializable]
    [DataContract]
    public abstract class AbstractCommandComponent : Component
    {
        protected virtual void BroadcastCommandment(Commandment commandment)
        {
            CommandMessage commandMessage = new CommandMessage(commandment, Parent?.Id);
            commandMessage.Emit();

            if(commandment == Commandment.MoveDown)
            {
                DropThroughPlatformRequest dropThroughPlatform = new DropThroughPlatformRequest {Target = Parent?.Id};
                dropThroughPlatform.Emit();
            }
        }
    }
}