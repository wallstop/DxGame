using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;

namespace DxCore.Core.Events
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

            return RuntimeHelpers.GetHashCode(this).CompareTo(RuntimeHelpers.GetHashCode(other));
        }
    }
}