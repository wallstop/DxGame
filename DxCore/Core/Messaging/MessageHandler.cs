﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private List<Action<Message>> acceptAllFunctions_;

        [DataMember] private GameId boundGameId_;

        [DataMember] private Dictionary<Type, object> handlersByType_;

        [DataMember] private Action<Message> wrongGameRejectionFunction_;

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public bool RegisterGlobally { get; set; } = true;

        [DataMember]
        private List<Action> Deregistrations { get; set; } = new List<Action>();

        [DataMember]
        private UniqueId Owner { get; set; }

        public MessageHandler(UniqueId ownerId)
        {
            Validate.Hard.IsNotNull(ownerId);
            Owner = ownerId;
            acceptAllFunctions_ = new List<Action<Message>>();
            handlersByType_ = new Dictionary<Type, object>();
        }

        /**
            Binds this MessageHandler to a specific Game Id. Messages that originates from any other GameId will be ignored. MessageHandlers can only be bound once.
        */

        public void BindToGame(GameId game, Action<Message> rejectionFunction)
        {
            Validate.Hard.IsNotNullOrDefault(game, $"Cannot bind a {nameof(MessageHandler)} to a null GameId");
            Validate.Hard.IsNull(boundGameId_,
                $"Cannot re-bind a {nameof(MessageHandler)} (already bound to {boundGameId_})");
            boundGameId_ = game;
            wrongGameRejectionFunction_ = rejectionFunction;
        }

        public void Deregister<T>() where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>(handlersByType_);
            if(!typedHandler.Handlers.Any())
            {
                Logger.Debug("Deregistering handler type {0} without any handlers", typeof(T));
            }
            typedHandler.Handlers.Clear();
            typedHandler.Deregistration?.Invoke();
            Deregistrations.Remove(typedHandler.Deregistration);
            typedHandler.Deregistration = null;
        }

        public void Deregister()
        {
            foreach(Action deregistration in Deregistrations)
            {
                deregistration?.Invoke();
            }
            Deregistrations.Clear();
        }

        public void HandleTypedMessage<T>(T message) where T : Message
        {
            if(!Active)
            {
                return;
            }
            if(!ShouldPropagate(message))
            {
                wrongGameRejectionFunction_.Invoke(message);
                return;
            }

            ActuallyHandleMessage(message);
        }

        public void HandleUntypedMessage(Message message)
        {
            if(!Active)
            {
                return;
            }
            if(!ShouldPropagate(message))
            {
                wrongGameRejectionFunction_.Invoke(message);
                return;
            }

            foreach(Action<Message> messageHandler in acceptAllFunctions_)
            {
                messageHandler.Invoke(message);
            }
        }

        public Action RegisterGlobalAcceptAll(Action<Message> acceptAllFunction)
        {
            acceptAllFunctions_.Add(acceptAllFunction);
            if(RegisterGlobally)
            {
                Action deregistration = GlobalMessageBus.RegisterGlobal(this);
                Deregistrations.Add(deregistration);
            }
            return new AcceptAllDeregistration(acceptAllFunction, acceptAllFunctions_).Deregister;
        }

        public Action RegisterMessageHandler<T>(Action<T> messageHandler) where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>(handlersByType_);
            typedHandler.Handlers.Add(messageHandler);
            if(RegisterGlobally)
            {
                Action deregistration = GlobalMessageBus.Register<T>(Owner, this);
                typedHandler.Deregistration = deregistration;
                Deregistrations.Add(deregistration);
            }

            /* We don't want to unsubscribe the message handler from the global bus - we only want to deregister this specific action from type handling */
            return new TypedHandlerDeregistration<T>(messageHandler, handlersByType_).Deregister;
        }

        public Action RegisterTargetedAcceptAll(Action<Message> acceptAllFunction)
        {
            acceptAllFunctions_.Add(acceptAllFunction);
            if(RegisterGlobally)
            {
                Action deregistration = GlobalMessageBus.RegisterTargetedGlobal(Owner, this);
                Deregistrations.Add(deregistration);
            }
            return new AcceptAllDeregistration(acceptAllFunction, acceptAllFunctions_).Deregister;
        }

        private void ActuallyHandleMessage<T>(T message) where T : Message
        {
            TypedHandler<T> typedHandler = HandlerForType<T>(handlersByType_);
            foreach(Action<T> handler in typedHandler.Handlers)
            {
                handler.Invoke(message);
            }
        }

        private static TypedHandler<T> HandlerForType<T>(Dictionary<Type, object> handlersByType) where T : Message
        {
            /* 
                TODO: Move to explicit mapping creation, right now, every time we handle a different message type, 
                we create a mapping on demand. Might be ok, but it never hurts to be extra sure :^)
            */
            object existingTypedHandler;
            Type type = typeof(T);
            if(handlersByType.TryGetValue(type, out existingTypedHandler))
            {
                return (TypedHandler<T>) existingTypedHandler;
            }

            TypedHandler<T> newTypedHandler = new TypedHandler<T>();
            handlersByType[type] = newTypedHandler;
            return newTypedHandler;
        }

        private bool ShouldPropagate<T>(T message) where T : Message
        {
            if(ReferenceEquals(boundGameId_, null))
            {
                return true;
            }

            GameId messageSource = message.GameId;
            return boundGameId_.Equals(messageSource);
        }

        [DataContract]
        [Serializable]
        internal class TypedHandler<T> where T : Message
        {
            public Action Deregistration { get; set; }
            public List<Action<T>> Handlers { get; } = new List<Action<T>>();
            public Action Registration { get; set; }
        }

        [DataContract]
        [Serializable]
        internal sealed class TypedHandlerDeregistration<T> where T : Message
        {
            [DataMember] private readonly Action<T> typedHandler_;

            [DataMember]
            private Dictionary<Type, object> HandlersByType { get; set; }

            private Action<T> TypedHandler => typedHandler_;

            public TypedHandlerDeregistration(Action<T> typedHandler, Dictionary<Type, object> handlersByType)
            {
                HandlersByType = handlersByType;
                typedHandler_ = typedHandler;
            }

            public void Deregister()
            {
                HandlerForType<T>(HandlersByType).Handlers.Remove(TypedHandler);
            }
        }
    }

    [DataContract]
    [Serializable]
    internal sealed class AcceptAllDeregistration
    {
        [DataMember]
        private List<Action<Message>> AcceptAllFunctions { get; set; }

        [DataMember]
        private Action<Message> Deregistration { get; set; }

        public AcceptAllDeregistration(Action<Message> deregistration, List<Action<Message>> acceptAllFunctions)
        {
            Deregistration = deregistration;
            AcceptAllFunctions = acceptAllFunctions;
        }

        public void Deregister()
        {
            AcceptAllFunctions.Remove(Deregistration);
        }
    }
}