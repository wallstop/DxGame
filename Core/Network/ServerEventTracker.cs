using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    [DataContract]
    [Serializable]
    public class ServerEventTracker
    {
        public MessageHandler Handler { get; } = new MessageHandler();

        private readonly List<Message> events_;

        public List<Message> RetrieveEvents()
        {
            List<Message> events = events_.ToList();
            events_.Clear();
            return events;
        }

        public ServerEventTracker()
        {
            events_ = new List<Message>();
            Handler.EnableAcceptAll(HandleMessage);
        }

        public ServerEventTracker(ServerEventTracker copy)
        {
            Validate.IsNotNullOrDefault(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            events_ = copy.events_.ToList();
            Handler.EnableAcceptAll(HandleMessage);
        }

        private void HandleMessage(Message message)
        {
            events_.Add(message);
        }
    }
}
