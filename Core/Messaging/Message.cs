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
    public class Message
    {
        [DataMember]
        public TimeSpan TimeStamp { get; set; }
        
        public static Message EmptyMessage { get; set; } = new Message(TimeSpan.Zero);

        protected Message() : this(DxGame.Instance.CurrentTime.TotalGameTime) {}

        protected Message(TimeSpan timeSpan)
        {
            TimeStamp = timeSpan;
        }

        public static void EmitTyped<T>(T message) where T : Message
        {
            DxGame.Instance.ProcessTypedMessage<T>(message);
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