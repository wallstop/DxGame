using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging
{
    public interface ITargetedMessage
    {
        UniqueId Target { get; }
    }
}
