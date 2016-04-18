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
    public sealed class MessageHandler
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        internal class TypedHandler<T> where T : Message
        {
            public List<Action<T>> Handlers { get; } = new List<Action<T>>();
            public Action Deregistration { get; set; }
        }

        [DataMember]
        private Dictionary<Type, object> handlersByType_;

        private TypedHandler<T> HandlerForType<T>() where T : Message
        {
            object existingTypedHandler;
            Type type = typeof(T);
            if(handlersByType_.TryGetValue(type, out existingTypedHandler))
            {
                return (TypedHandler<T>) existingTypedHandler;
            }

            TypedHandler<T> newTypedHandler = new TypedHandler<T>();
            handlersByType_[type] = newTypedHandler;
            return newTypedHandler;
        }

        [DataMember]
        private List<Action> Deregistrations { get; set; } = new List<Action>();

        [DataMember]
        private UniqueId Owner { get; set; }

        [DataMember] private List<Action<Message>> acceptAllFunctions_;

        public MessageHandler(UniqueId ownerId)
        {
            Validate.IsNotNull(ownerId);
            Owner = ownerId;
            acceptAllFunctions_ = new List<Action<Message>>();
            handlersByType_ = new Dictionary<Type, object>();
        }

        public Action RegisterTargetedAcceptAll(Action<Message> acceptAllFunction)
        {
            acceptAllFunctions_.Add(acceptAllFunction);
            Action deregistration = GlobalMessageBus.RegisterTargetedGlobal(Owner, this);
            Deregistrations.Add(deregistration);
            return () => acceptAllFunctions_.Remove(acceptAllFunction);
        }

        public Action RegisterGlobalAcceptAll(Action<Message> acceptAllFunction)
        {
            acceptAllFunctions_.Add(acceptAllFunction);
            Action deregistration = GlobalMessageBus.RegisterGlobal(this);
            Deregistrations.Add(deregistration);
            return () => acceptAllFunctions_.Remove(acceptAllFunction);
        }

        public Action RegisterMessageHandler<T>(Action<T> messageHandler) where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>();
            typedHandler.Handlers.Add(messageHandler);
            Action deregistration = GlobalMessageBus.Register<T>(Owner, this);
            typedHandler.Deregistration = deregistration;
            Deregistrations.Add(deregistration);

            /* We don't want to unsubscribe the message handler from the global bus - we only want to deregister this specific action from type handling */
            return () => typedHandler.Handlers.Remove(messageHandler);
        }

        public void Deregister<T>() where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>();
            if(!typedHandler.Handlers.Any())
            {
                LOG.Debug("Deregistering handler type {0} without any handlers", typeof(T));
            }
            typedHandler.Handlers.Clear();
            typedHandler.Deregistration.Invoke();
            Deregistrations.Remove(typedHandler.Deregistration);
            typedHandler.Deregistration = null;
        }

        public void Deregister()
        {
            foreach(Action deregistration in Deregistrations)
            {
                deregistration.Invoke();
            }
            Deregistrations.Clear();
        }

        public void HandleTypedMessage<T>(T message) where T : Message
        {
                ActuallyHandleMessage(message);
        }

        public void HandleGlobalMessage(Message message)
        {
            foreach(Action<Message> messageHandler in acceptAllFunctions_)
            {
                messageHandler.Invoke(message);
            }
        }

        private void ActuallyHandleMessage<T>(T message) where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>();
            foreach(Action<T> handler in typedHandler.Handlers)
            {
                handler.Invoke(message);
            }
        }
    }
}