using System;
using System.Runtime.Serialization;
using DXGame.Main;

namespace DXGame.Core.Messaging
{
    /**
        <summary>
            Simple base class for all of your messaging needs :^)

            TODO: Make interface? Abstract? Core... message ...methods? This isn't ideal.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Message : IIdentifiable
    {
        [DataMember]
        public TimeSpan TimeStamp { get; }

        public virtual bool Global => false;

        public static Message EmptyMessage { get; } = new Message(TimeSpan.Zero);

        protected Message() : this(DxGame.Instance.CurrentTime.TotalGameTime) {}

        protected Message(TimeSpan timeSpan)
        {
            Id = new UniqueId();
            TimeStamp = timeSpan;
        }

        [DataMember]
        public UniqueId Id { get; }
    }
}