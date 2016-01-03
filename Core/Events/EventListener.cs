using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DXGame.Core.Events
{
    /**
        <summary>
            Simple Wrapper that allows the decoupling of Event listeners (observers) and the Event Model
        </summary>
    */

    [DataContract]
    [Serializable]
    public class EventListener
    {
        public Action<Event> EventConsumer { get; }

        public EventListener(Action<Event> eventConsumer)
        {
            Validate.IsNotNullOrDefault(eventConsumer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, "EventConsumer"));
            EventConsumer = eventConsumer;
        }

        public void OnEvent(Event triggeredEvent)
        {
            EventConsumer(triggeredEvent);
        }
    }
}