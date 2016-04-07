using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Command
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
            switch(commandment)
            {
                case Commandment.MoveDown:
                {
                    Parent?.BroadcastTypedMessage(new CommandMessage(commandment));
                    Parent?.BroadcastTypedMessage(new DropThroughPlatformRequest());
                }
                    break;
                case Commandment.InteractWithEnvironment:
                {
                    Parent?.BroadcastTypedMessage(new CommandMessage(commandment));
                    // TODO: Move somewhere else?
                    DxGame.Instance.BroadcastTypedMessage(new EnvironmentInteractionMessage {Source = Parent});
                }
                    break;
                default:
                    Parent?.BroadcastTypedMessage(new CommandMessage(commandment));
                    break;
            }
        }
    }
}