using System;
using System.Collections.Generic;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.Messaging
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

        private static Dictionary<UniqueId, MessageHandler> UntargetedSinks { get; } = new Dictionary<UniqueId, MessageHandler>();
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
            return UntargetedSinks.TryGetValue(target, out handler);
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

            HashSet<MessageHandler> handlersForType = Handler<T>();
            handlersForType.Add(messageHandler);

            Dictionary<UniqueId, MessageHandler> targetedHandlers = TargetedHandlers<T>();
            targetedHandlers[handlerOwnerId] = messageHandler;
            return () =>
            {
                handlersForType.Remove(messageHandler);
                targetedHandlers.Remove(handlerOwnerId);
            };
        }

        public static Action RegisterTargetedGlobal(UniqueId handlerOwnerId, MessageHandler messageHandler)
        {
            UntargetedSinks[handlerOwnerId] = messageHandler;
            return () => UntargetedSinks.Remove(handlerOwnerId);
        }

        public static Action RegisterGlobal(MessageHandler messageHandler)
        {
            GlobalSinks.Add(messageHandler);
            return () => GlobalSinks.Remove(messageHandler);
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
            foreach(MessageHandler handler in GlobalSinks)
            {
                handler.HandleGlobalMessage(typedMessage);
            }
        }

        private static void BroadcastGlobal<T>(T typedMessage) where T : Message
        {
            foreach(MessageHandler handler in GlobalSinks)
            {
                handler.HandleGlobalMessage(typedMessage);
            }
        }

        private static void TargetedBroadcast<T>(UniqueId target, T typedAndTargetedMessage) where T : Message
        {
            MessageHandler handler;
            if(UntargetedHandler(target, out handler))
            {
                handler.HandleGlobalMessage(typedAndTargetedMessage);
                //return;
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
