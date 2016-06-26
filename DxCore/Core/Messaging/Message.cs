using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Messaging
{
    /**
        <summary>
            Simple base class for all of your messaging needs :^)
        </summary>
    */

    [Serializable]
    [DataContract]
    public class Message
    {
        [DataMember]
        public TimeSpan TimeStamp { get; private set; }

        [DataMember]
        public GameId GameId { get; private set; }

        public static Message EmptyMessage { get; set; } = new Message(TimeSpan.Zero, GameId.Empty);

        protected Message() : this(DxGame.Instance.CurrentTime.TotalGameTime, DxGame.Instance.GameGuid) {}

        protected Message(TimeSpan timeSpan, GameId gameId)
        {
            TimeStamp = timeSpan;
            GameId = gameId;
        }

        public static void EmitTyped<T>(T message) where T : Message
        {
            DxGame.Instance.ProcessTypedMessage(message);
        }

        public void EmitUntyped()
        {
            DxGame.Instance.ProcessUntypedMessage(this);
        }
    }

    public static class MessageExtensions
    {
        public static void Emit<T>(this T message) where T : Message
        {
            Message.EmitTyped(message);
        }
    }
}