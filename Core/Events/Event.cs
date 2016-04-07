using System;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Events
{
    /**
        <summary>
            Events are lightweight wrappers around Messages, describing a Message and when it occurred. 
        </summary>
    */
    [Serializable]
    [DataContract]
    public class Event : IComparable<Event>
    {
        public static Event NullEventFor(TimeSpan timeSpan)
        {
            return new Event(timeSpan);
        }

        private Event(TimeSpan timeSpan)
        {
            Message = Message.EmptyMessage;
            GameTime = new DxGameTime(timeSpan, DxGame.Instance.TargetElapsedTime);
        }

        public Event(Message message, DxGameTime gameTime)
        {
            Validate.IsNotNull(message);
            Validate.IsNotNull(gameTime);
            Message = message; // TODO: Copy?
            GameTime = gameTime;
        }

        [DataMember]
        public Message Message { get; }

        [DataMember]
        public DxGameTime GameTime { get; }

        public int CompareTo(Event other)
        {
            if(ReferenceEquals(other, null))
            {
                return 1;
            }
            int timeComparison = GameTime.CompareTo(other.GameTime);
            if(timeComparison != 0)
            {
                return timeComparison;
            }
            int messageComparison = Message.Id.CompareTo(other.Message.Id);
            return messageComparison;
        }
    }
}