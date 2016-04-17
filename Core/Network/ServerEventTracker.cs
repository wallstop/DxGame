using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    [DataContract]
    [Serializable]
    public class ServerEventTracker
    {
        public MessageHandler Handler { get; }

        private readonly List<Message> events_;

        private readonly List<NetworkMessage> clientSpecificMessages_;

        private readonly UniqueId trackerId_;

        public bool RetrieveNetworkEvents(out List<NetworkMessage> clientSpecificMessages)
        {
            if(!clientSpecificMessages_.Any())
            {
                clientSpecificMessages = null;
                return false;
            }
            clientSpecificMessages = clientSpecificMessages_.ToList();
            clientSpecificMessages_.Clear();
            return true;
        }

        public void AttachNetworkMessage(NetworkMessage networkMessage)
        {
            clientSpecificMessages_.Add(networkMessage);
        }

        public List<Message> RetrieveEvents()
        {
            List<Message> events = events_.ToList();
            events_.Clear();
            return events;
        }

        public ServerEventTracker()
        {
            events_ = new List<Message>();
            Handler.EnableGlobalAcceptAll(HandleMessage);
            clientSpecificMessages_ = new List<NetworkMessage>();
            trackerId_ = new UniqueId();
            Handler = new MessageHandler(trackerId_);
        }

        public ServerEventTracker(ServerEventTracker copy)
        {
            Validate.IsNotNullOrDefault(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            events_ = copy.events_.ToList();
            clientSpecificMessages_ = new List<NetworkMessage>();
            Handler.EnableGlobalAcceptAll(HandleMessage);
            trackerId_ = new UniqueId();
            Handler = new MessageHandler(trackerId_);
        }

        private void HandleMessage(Message message)
        {
            events_.Add(message);
        }
    }
}
