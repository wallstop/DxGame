using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.Messaging
{
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

        [DataMember] private readonly Dictionary<Type, List<dynamic>> typesToMessageHandlers_ =
            new Dictionary<Type, List<dynamic>>();

        [DataMember]
        public bool AcceptAll { get; private set; }

        public void EnableAcceptAll()
        {
            Validate.IsFalse(AcceptAll, $"Expected {nameof(AcceptAll)} to be false, but it was not :(");
            LOG.Info("Enabling Accept-All mode");
            AcceptAll = true;
        }

        public void RegisterMessageHandler<T>(Action<T> messageHandler) where T : Message
        {
            Type type = typeof(T);
            List<object> existingHandlers = ExistingMessageHandlers(type);
            Validate.IsFalse(existingHandlers.Contains(messageHandler),
                StringUtils.GetFormattedAlreadyContainsMessage(this, messageHandler, existingHandlers));
            existingHandlers.Add(messageHandler);
            typesToMessageHandlers_[type] = existingHandlers;
        }

        private List<dynamic> ExistingMessageHandlers(Type type)
        {
            return (typesToMessageHandlers_.ContainsKey(type) ? typesToMessageHandlers_[type] : new List<dynamic>());
        }

        public void DeregisterMessageHandlers(Type type)
        {
            List<object> existingHandlers = ExistingMessageHandlers(type);
            existingHandlers.Clear();
        }

        public void DeregisterMessageHandler<T>(Action<T> messageHandler) where T : Message
        {
            Type type = typeof(T);
            List<dynamic> existingMessageHandlers = ExistingMessageHandlers(type);
            Validate.IsTrue(existingMessageHandlers.Contains(messageHandler),
                $"Cannot remove {typeof(MessageHandler)} {messageHandler}, as it is not associated with the provided type");
            existingMessageHandlers.Remove(messageHandler);
        }

        public void HandleUntypedMessage(Message message)
        {
            dynamic trueMessageType = message;
            InternalHandleMessage(trueMessageType);
        }

        public void HandleTypedMessage<T>(T message) where T : Message
        {
            InternalHandleMessage(message);
        }

        private void InternalHandleMessage<T>(T message) where T : Message
        {
            if(AcceptAll)
            {
                SpamMessageHandlers(message);
            }
            else
            {
                ActuallyHandleMessage(message);
            }
        }

        private void SpamMessageHandlers<T>(T message) where T : Message
        {
            foreach(var messageHandler in typesToMessageHandlers_.Values.SelectMany(values => values))
            {
                ((Action<T>) (messageHandler))(message);
            }
        }

        private void ActuallyHandleMessage<T>(T message) where T : Message
        {
            var type = message.GetType();
            if(!typesToMessageHandlers_.ContainsKey(type))
            {
                return;
            }

            IEnumerable<object> messageHandlers = typesToMessageHandlers_[type];
            foreach(var messageHandler in messageHandlers)
            {
                /* They're really TypedMessageHandlers, I promise! */
                ((Action<T>) (messageHandler))(message);
            }
        }
    }
}