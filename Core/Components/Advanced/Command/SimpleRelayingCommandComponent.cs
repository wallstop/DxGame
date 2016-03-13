using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;

namespace DXGame.Core.Components.Advanced.Command
{
    [DataContract]
    [Serializable]
    public class SimpleRelayingCommandComponent : AbstractCommandComponent
    {
        public void RelayCommands(List<Commandment> commandments)
        {
            foreach(Commandment commandment in commandments)
            {
                BroadcastCommandment(commandment);
            }
        }
    }
}
