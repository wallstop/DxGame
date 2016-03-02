using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Network;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Network
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
