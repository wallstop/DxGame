using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Utils;
using MemoryCache = System.Runtime.Caching.MemoryCache;
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
        private readonly ObjectCache messageCache_;

        public ServerEventTracker()
        {
            messageCache_ = NewObjectCache;
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
            messageCache_ = NewObjectCache;
        }

        public bool StopTracking(Message message)
        {
            //messageCache_.Remove
            //cache_.
            //MemoryCache.Default.Remove(uniqueIdentifier_, null)
            //return eventQueue_.Remove(message);
            return true;
        }

        private static ObjectCache NewObjectCache
        {
            get
            {
                //MemoryCache<Message> b = new MemoryCache<Message>("");
                // TODO PLS GOOD CACHE, MemoryCache is garbage. Look into porting Caffeine (https://github.com/ben-manes/caffeine)
                string uniqueCacheIdentifier = Guid.NewGuid().ToString();
                return new System.Runtime.Caching.MemoryCache(uniqueCacheIdentifier);
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
