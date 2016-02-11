using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Utils;
using FlitBit.Cache;
using Message = DXGame.Core.Messaging.Message;

namespace DXGame.Core.Network
{
    [DataContract]
    [Serializable]
    public class ServerEventTracker
    {
        public MessageHandler Handler { get; } = new MessageHandler();

        // TODO: Hand off this responsibility to someone else
        private static readonly Predicate<object> NON_SERIALIZATION_CHECK = entity =>
        {
            var component = entity as Component;
            return component != null && !component.ShouldSerialize;
        };

        private readonly List<Message> events_;

        public ServerEventTracker()
        {
            events_ = new List<Message>();
            Handler.EnableAcceptAll();
            Handler.RegisterMessageHandler<Message>(HandleMessage);
        }

        public ServerEventTracker(ServerEventTracker copy)
        {
            Validate.IsNotNullOrDefault(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            lock (copy.events_)
            {
                events_ = copy.events_.ToList();
            }
        }

        public bool StopTracking(Message message)
        {
            //cache_.
            //MemoryCache.Default.Remove(uniqueIdentifier_, null)
            //return eventQueue_.Remove(message);
            return true;
        }

        private static object NewObjectCache
        {
            get
            {
                // TODO PLS GOOD CACHE
                string uniqueCacheIdentifier = Guid.NewGuid().ToString();
                return null;
            }
        }

        //public List<Message>  

        private void HandleMessage(Message message)
        {
            // TODO: Figure out a way to... delegate responsibility of which events should be serialized
            // For now, hard code it and go with the flow

            EntityStatusChangedMessage entityStatusChanged = message as EntityStatusChangedMessage;
            if(ReferenceEquals(entityStatusChanged, null))
            {
                return;
            }

            // Hey, status changed, we care, pop it in the queue
            //events.Add(entityStatusChanged);
        }
    }
}
