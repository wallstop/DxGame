using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Network
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
            trackerId_ = new UniqueId();
            Handler = new MessageHandler(trackerId_);
            Handler.RegisterGlobalAcceptAll(HandleMessage);
            clientSpecificMessages_ = new List<NetworkMessage>();
        }

        public ServerEventTracker(ServerEventTracker copy)
        {
            Validate.IsNotNullOrDefault(copy, this.GetFormattedNullOrDefaultMessage(copy));
            events_ = copy.events_.ToList();
            clientSpecificMessages_ = new List<NetworkMessage>();
            trackerId_ = new UniqueId();
            Handler = new MessageHandler(trackerId_);
            Handler.RegisterGlobalAcceptAll(HandleMessage);
        }

        private void HandleMessage(Message message)
        {
            events_.Add(message);
        }
    }
}
