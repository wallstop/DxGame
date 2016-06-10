﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Network;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Network
{
    [DataContract]
    [Serializable]
    public class EventStream : NetworkMessage
    {
        [DataMember]
        public List<Message> Messages { get; set; } = new List<Message>();

        public EventStream() {}

        public EventStream(List<Message> messages)
        {
            Validate.IsNotNull(messages);
            Messages = messages.ToList();
        }
    }
}
