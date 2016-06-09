using DXGame.Core;

namespace DxCore.Core.Messaging
{
    public interface ITargetedMessage
    {
        UniqueId Target { get; }
    }
}
