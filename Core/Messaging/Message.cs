using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class Message : IIdentifiable
    {
        public virtual UniqueId Id { get; } = new UniqueId();
    }
}