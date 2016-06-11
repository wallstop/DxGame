using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Events
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
            Validate.IsNotNullOrDefault(eventConsumer, this.GetFormattedNullOrDefaultMessage("EventConsumer"));
            EventConsumer = eventConsumer;
        }

        public void OnEvent(Event triggeredEvent)
        {
            EventConsumer(triggeredEvent);
        }
    }
}