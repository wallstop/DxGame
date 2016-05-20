namespace DXGame.Core.Messaging
{
    public interface ITargetedMessage
    {
        UniqueId Target { get; }
    }
}
