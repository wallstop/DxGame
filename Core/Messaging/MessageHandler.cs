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
            handlersByType_.
            if(handlersByType_.)
            // TODO
        }


        //internal class TypeHandlerResolver
        //{
        //    Lazy<TypedHandler<T>> h = new Lazy<TypedHandler<T>>(() => new TypedHandler<T>());
        //    public TypedHandler<T> Resolve<T>() where T : Message
        //    {
        //        return h.Value;
        //    }
        //}



        [DataMember]
        private List<Action> Deregistrations { get; set; } = new List<Action>();

        [DataMember] private Action<Message> acceptAllFunction_;

        [DataMember]
        public bool AcceptAll { get; private set; }

        [DataMember]
        public UniqueId Owner { get; private set; }

        public MessageHandler(UniqueId ownerId)
        {
            Validate.IsNotNull(ownerId);
            Owner = ownerId;
            handlersByType_ = new Dictionary<Type, object>();
        }

        public void EnableTargetedAcceptAll(Action<Message> acceptAllFunction)
        {
            Validate.IsFalse(AcceptAll, $"Expected {nameof(AcceptAll)} to be false, but it was not :(");
            LOG.Info("Enabling Accept-All mode for {0}", Owner);
            AcceptAll = true;
            acceptAllFunction_ = acceptAllFunction;
            Action deregistration = GlobalMessageBus.RegisterTargetedGlobal(Owner, this);
            Deregistrations.Add(deregistration);
        }

        public void EnableGlobalAcceptAll(Action<Message> acceptAllFunction)
        {
            Validate.IsFalse(AcceptAll, $"Expected {nameof(AcceptAll)} to be false, but it was not :(");
            LOG.Info("Enabling Accept-All mode for {0}", Owner);
            AcceptAll = true;
            acceptAllFunction_ = acceptAllFunction;
            Action deregistration = GlobalMessageBus.RegisterGlobal(this);
            Deregistrations.Add(deregistration);
        }

        public void RegisterMessageHandler<T>(Action<T> messageHandler) where T : Message
        {
            TypedHandler<T>.Handlers.Add(messageHandler);
            Action deregistration = GlobalMessageBus.Register<T>(Owner, this);
            TypedHandler<T>.Deregistration = deregistration;
            Deregistrations.Add(deregistration);
        }

        public void Deregister<T>() where T : Message
        {
            if(!TypedHandler<T>.Handlers.Any())
            {
                LOG.Debug("Deregistering handler type {0} without any handlers", typeof(T));
            }
            TypedHandler<T>.Handlers.Clear();
            TypedHandler<T>.Deregistration.Invoke();
            Deregistrations.Remove(TypedHandler<T>.Deregistration);
            TypedHandler<T>.Deregistration = null;
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
            if(AcceptAll)
            {
                acceptAllFunction_.Invoke(message);
            }
            else
            {
                ActuallyHandleMessage(message);
            }
        }

        private void ActuallyHandleMessage<T>(T message) where T : Message
        {
            foreach(Action<T> handler in TypedHandler<T>.Handlers)
            {
                handler.Invoke(message);
            }
        }
    }
}