using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Utils;
using NLog;

namespace DxCore.Core.Messaging
{
    public static class GlobalMessageBus
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private static class SpecializedHandler<T> where T : Message
        {
            public static HashSet<MessageHandler> Sinks { get; } = new HashSet<MessageHandler>();

            public static Dictionary<UniqueId, MessageHandler> TargetedSinks { get; } =
                new Dictionary<UniqueId, MessageHandler>();
        }

        [Serializable]
        [DataContract]
        internal sealed class SerializableDeregistration<T> where T : Message
        {
            [DataMember]
            private UniqueId Target { get; set; }

            public SerializableDeregistration(UniqueId target)
            {
                Target = target;
            }

            public void Deregister()
            {
                Dictionary<UniqueId, MessageHandler> targetedHandlers = TargetedHandlers<T>();
                MessageHandler messageHandler;
                if(targetedHandlers.TryGetValue(Target, out messageHandler))
                {
                    Handler<T>().Remove(messageHandler);
                }
                targetedHandlers.Remove(Target);
            }
        }

        [Serializable]
        [DataContract]
        internal sealed class SerializableUntypedTargetedDeregistration
        {
            [DataMember]
            private UniqueId Target { get; set; }

            public SerializableUntypedTargetedDeregistration(UniqueId target)
            {
                Target = target;
            }

            public void DeregisterUntypedTargeted()
            {
                UntypedTargetedSinks.Remove(Target);
            }
        }

        [Serializable]
        [DataContract]
        internal sealed class SerializableUntypedGlobalDeregistration
        {
            [DataMember]
            private MessageHandler Handler { get; set; }

            public SerializableUntypedGlobalDeregistration(MessageHandler handler)
            {
                Handler = handler;
            }

            public void DeregisterUntypedGlobal()
            {
                GlobalSinks.Remove(Handler);
            }
        }

        private static Dictionary<UniqueId, MessageHandler> UntypedTargetedSinks { get; } = new Dictionary<UniqueId, MessageHandler>();
        private static HashSet<MessageHandler> GlobalSinks { get; } = new HashSet<MessageHandler>();

        private static HashSet<MessageHandler> Handler<T>() where T : Message
        {
            return SpecializedHandler<T>.Sinks;
        }

        private static Dictionary<UniqueId, MessageHandler> TargetedHandlers<T>() where T : Message
        {
            return SpecializedHandler<T>.TargetedSinks;
        }

        private static bool TargetedHandler<T>(UniqueId target, out MessageHandler handler) where T : Message
        {
            return SpecializedHandler<T>.TargetedSinks.TryGetValue(target, out handler);
        }

        private static bool UntargetedHandler(UniqueId target, out MessageHandler handler)
        {
            return UntypedTargetedSinks.TryGetValue(target, out handler);
        }

        public static void UntypedBroadcast(Message message)
        {
            dynamic typedMessage = message;
            TypedBroadcast(typedMessage);
        }

        /*
            Registers the provided HandlerId and MessageHandler with the specified type, providing a deregistration function to call on 
            deregistration 
        */

        public static Action Register<T>(UniqueId handlerOwnerId, MessageHandler messageHandler) where T : Message
        {
            Validate.IsNotNullOrDefault(handlerOwnerId);
            Validate.IsNotNullOrDefault(messageHandler);

            Dictionary<UniqueId, MessageHandler> targetedHandlers = TargetedHandlers<T>();
            MessageHandler existingHandler;
            if(!targetedHandlers.TryGetValue(handlerOwnerId, out existingHandler))
            {
                targetedHandlers[handlerOwnerId] = messageHandler;

            }
            else if(!ReferenceEquals(existingHandler, messageHandler))
            {
                LOG.Warn(
                    "Ignoring double registration of {0} with different handlers (is this intentional? Likely a bug)",
                    handlerOwnerId);
                return new SerializableDeregistration<T>(handlerOwnerId).Deregister;
            }

            HashSet<MessageHandler> handlersForType = Handler<T>();
            bool newRegistration = handlersForType.Add(messageHandler);
            if(!newRegistration)
            {
                LOG.Debug("Received double registration of {0} for {1}", typeof(T), handlerOwnerId);
            }

            return new SerializableDeregistration<T>(handlerOwnerId).Deregister;
        }

        /* TODO: This naming convention is garbage, please redo. No one is going to understand what any of this crap means */

            /**
                <summary>
                    Registers an untyped message handler for all messages that are for the specified target
                </summary>   
            */
        public static Action RegisterTargetedGlobal(UniqueId handlerOwnerId, MessageHandler messageHandler)
        {
            if(UntypedTargetedSinks.ContainsKey(handlerOwnerId))
            {
                LOG.Info("Ignoring double targeted global registration of {0}", handlerOwnerId);
                return new SerializableUntypedTargetedDeregistration(handlerOwnerId).DeregisterUntypedTargeted;
            }

            UntypedTargetedSinks[handlerOwnerId] = messageHandler;
            return new SerializableUntypedTargetedDeregistration(handlerOwnerId).DeregisterUntypedTargeted;
        }

        public static Action RegisterGlobal(MessageHandler messageHandler)
        {
            GlobalSinks.Add(messageHandler);
            return new SerializableUntypedGlobalDeregistration(messageHandler).DeregisterUntypedGlobal;
        }

        public static void TypedBroadcast<T>(T typedMessage) where T : Message
        {
            BroadcastGlobal<T>(typedMessage);
            ITargetedMessage targetedMessage = typedMessage as ITargetedMessage;
            if(!ReferenceEquals(targetedMessage, null))
            {
                UniqueId target = targetedMessage.Target;
                TargetedBroadcast(target, typedMessage);
                return;
            }
            UntargetedBroadcast(typedMessage);
        }

        private static void UntargetedBroadcast<T>(T typedMessage) where T : Message
        {
            foreach(MessageHandler handler in Handler<T>())
            {
                handler.HandleTypedMessage(typedMessage);
            }
            BroadcastGlobal<T>(typedMessage);
        }

        private static void BroadcastGlobal<T>(T typedMessage) where T : Message
        {
            foreach(MessageHandler handler in GlobalSinks)
            {
                handler.HandleUntypedMessage(typedMessage);
            }
        }

        private static void TargetedBroadcast<T>(UniqueId target, T typedAndTargetedMessage) where T : Message
        {
            MessageHandler handler;
            if(UntargetedHandler(target, out handler))
            {
                handler.HandleUntypedMessage(typedAndTargetedMessage);
            }

            /* Re-use existing handler */
            if(TargetedHandler<T>(target, out handler))
            {
                handler.HandleTypedMessage(typedAndTargetedMessage);
                return;
            }

            LOG.Info("Could not find a matching handler for Id: {0}, Message: {1}", target, typedAndTargetedMessage);
        }
    }
}
