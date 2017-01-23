using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Input;
using DxCore.Core.Network;
using DxCore.Core.Utils;
using WallNetCore.Validate;

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
            Validate.Hard.IsNotNullOrDefault(clientKeyboardEvents,
                () => this.GetFormattedNullOrDefaultMessage(nameof(clientKeyboardEvents)));
            ClientKeyboardEvents = clientKeyboardEvents;
        }
    }
}