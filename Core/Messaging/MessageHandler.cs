using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

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
        [DataMember] private readonly Dictionary<Type, List<object>> typesToMessageHandlers_ =
            new Dictionary<Type, List<object>>();

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
            var type = typeof (T);
            if (!typesToMessageHandlers_.ContainsKey(type))
            {
                return;
            }

            IEnumerable<object> messageHandlers = typesToMessageHandlers_[type];
            foreach (var messageHandler in messageHandlers)
            {
                /* They're really TypedMessageHandlers, I promise! */
                ((TypedMessageHandler<T>) (messageHandler))(message);
            }
        }
    }
}