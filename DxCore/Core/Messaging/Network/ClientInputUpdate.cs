using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Input;
using DxCore.Core.Network;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class ClientInputUpdate : NetworkMessage
    {
        [DataMember]
        public List<KeyboardEvent> ClientKeyboardEvents { get; private set; }

        public ClientInputUpdate(List<KeyboardEvent> clientKeyboardEvents)
        {
            Validate.IsNotNullOrDefault(clientKeyboardEvents,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(clientKeyboardEvents)));
            ClientKeyboardEvents = clientKeyboardEvents;
        }
    }
}
