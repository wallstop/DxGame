using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.Messaging
{
    public delegate void TypedMessageHandler<in T>(T message) where T : Message;


    /**
        <summary>
            Abstraction layer for immediate-mode Message passing. An instance of this handles all
            kinds of types to trigger functions that are registered with it.
        </summary>

    */

    [Serializable]
    [DataContract]
    public class MessageHandler
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly Dictionary<Type, List<object>> typesToMessageHandlers_ =
            new Dictionary<Type, List<object>>();

        [DataMember]
        public bool AcceptAll { get; private set; } = false;

        public void EnableAcceptAll()
        {
            Validate.IsFalse(AcceptAll, $"Expected {nameof(AcceptAll)} to be false, but it was not :(");
            LOG.Info($"Enabling Accept-All mode");
            AcceptAll = true;
        }

        public void RegisterMessageHandler<T>(TypedMessageHandler<T> messageHandler) where T : Message
        {
            var type = typeof (T);
            Validate.IsNotNull(type, StringUtils.GetFormattedNullOrDefaultMessage(this, type));
            List<object> existingHandlers = ExistingMessageHandlers(type);
            Validate.IsFalse(existingHandlers.Contains(messageHandler),
                StringUtils.GetFormattedAlreadyContainsMessage(this, messageHandler, existingHandlers));
            existingHandlers.Add(messageHandler);
            typesToMessageHandlers_[type] = existingHandlers;
        }

        private List<object> ExistingMessageHandlers(Type type)
        {
            return (typesToMessageHandlers_.ContainsKey(type)
                ? typesToMessageHandlers_[type]
                : new List<object>());
        }

        public void DeregisterMessageHandlers(Type type)
        {
            List<object> existingHandlers = ExistingMessageHandlers(type);
            existingHandlers.Clear();
        }

        public void DeregisterMessageHandler<T>(TypedMessageHandler<T> messageHandler) where T : Message
        {
            var type = typeof (T);
            Validate.IsNotNull(type, StringUtils.GetFormattedNullOrDefaultMessage(this, type));
            List<object> existingMessageHandlers = ExistingMessageHandlers(type);
            Validate.IsTrue(existingMessageHandlers.Contains(messageHandler),
                $"Cannot remove {typeof (MessageHandler)} {messageHandler}, as it is not associated with the provided type");
            existingMessageHandlers.Remove(messageHandler);
        }

        public void HandleMessage<T>(T message) where T : Message
        {
            if(AcceptAll)
            {
                SpamMessageHandlers<T>(message);
            }
            else
            {
                ActuallyHandleMessage<T>(message);
            }
        }

        private void SpamMessageHandlers<T>(T message) where T : Message
        {
            foreach(object messageHandler in typesToMessageHandlers_.Values.SelectMany(values => values))
            {
                ((TypedMessageHandler<T>) (messageHandler))(message);
            }
        }

        private void ActuallyHandleMessage<T>(T message) where T : Message
        {
            var type = typeof(T);
            if(!typesToMessageHandlers_.ContainsKey(type))
            {
                return;
            }

            IEnumerable<object> messageHandlers = typesToMessageHandlers_[type];
            foreach(var messageHandler in messageHandlers)
            {
                /* They're really TypedMessageHandlers, I promise! */
                ((TypedMessageHandler<T>) (messageHandler))(message);
            }
        }
    }
}