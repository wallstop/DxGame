﻿namespace DxCore.Core.Messaging
{
    public interface ITypedMessageProcessor
    {
        void ProcessTypedMessage<T>(T message) where T : Message;
    }
}
