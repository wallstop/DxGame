using DXGame.Core.Messaging;

namespace DxCore.Core.Messaging
{
    public interface IUntypedMessageProcessor
    {
        void ProcessUntypedMessage(Message message);
    }
}
