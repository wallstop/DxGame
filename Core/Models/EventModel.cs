using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.DataStructures;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Models
{
    /**

        <summary>
            Global Message-pump for all messages that are broadcast to the game. 
            Maintains messages for some time period and provides a queryable interface
        </summary>
    */
    [Serializable]
    [DataContract]
    public class EventModel : Model
    {
        public static TimeSpan Expiry { get; } = TimeSpan.FromSeconds(5);

        [DataMember]
        private readonly SortedList<Event> events_ = new SortedList<Event>();

        public EventModel()
        {
            MessageHandler.EnableAcceptAll();
            MessageHandler.RegisterMessageHandler<Message>(HandleMessage);
        }

        private void HandleMessage(Message message)
        {
            DxGameTime currentGameTime = DxGame.Instance.CurrentTime;
            Event gameEvent = new Event(message, currentGameTime);
            events_.Add(gameEvent);
        }

        protected override void Update(DxGameTime gameTime)
        {
            Event cutoffPoint = Event.NullEventFor(gameTime.TotalGameTime);
            events_.RemoveBelow(cutoffPoint);
            base.Update(gameTime);
        }

        /**

            <summary>
                Returns an unfiltered view of every kept event. 
                Modifying this list will not modify the Event Model's backing list.
            </summary>
        */
        public List<Event> AllEvents => events_.ToList(); 

        /**
            <summary>
                Returns all Events that have Messages of the provided type
            </summary>
        */
        public List<Event> EventsOfType<T>() where T : Message
        {
            return events_.Where(gameEvent => gameEvent.Message is T).ToList();
        }

        /**
            <summary>
                Returns all Event within some timespan. 
                This timespan must be less than or equal to the Expiry defined by the event model.
            </summary>
        */
        public List<Event> EventsWithin(TimeSpan timeSpan, DxGameTime gameTime)
        {
            Validate.IsTrue(timeSpan <= Expiry, $"{timeSpan} was longer than our expiry ({Expiry})");
            return
                events_.Where(gameEvent => gameTime.TotalGameTime - timeSpan >= gameEvent.GameTime.TotalGameTime)
                    .ToList();
        } 

        /**
            <summary>
                Returns all Events that match the request's types and timeframe.
            </summary>
        */
        public List<Event> EventsFor(EventRequest request, DxGameTime gameTime)
        {
            Validate.IsNotNull(request, $"Cannot retrieve {typeof(Event)}s for a null {typeof(EventRequest)}");
            TimeSpan cutoff = gameTime.TotalGameTime - request.Cutoff;
            return
                events_.Where(
                    gameEvent =>
                        request.Types.Contains(gameEvent.Message.GetType()) &&
                        gameEvent.GameTime.TotalGameTime >= cutoff).ToList();
        } 
    }

    /**
        <summary>
            Utility request builder for when you need more than a simple OfType or (within some time range) style 
            request for EventModel.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class EventRequest
    {
        [DataMember]
        public IReadOnlyCollection<Type> Types { get; }

        [DataMember]
        public TimeSpan Cutoff { get;  }

        private EventRequest(IEnumerable<Type> types, TimeSpan cutoff)
        {
            Types = new ReadOnlyCollection<Type>(types.ToList());
            Cutoff = cutoff;
        } 

        public static EventRequestBuilder Builder()
        {
            return new EventRequestBuilder();
        }

        public class EventRequestBuilder
        {
            private readonly HashSet<Type> types_ = new HashSet<Type>();
            private TimeSpan cutoff_ = EventModel.Expiry;

            public EventRequestBuilder WithType<T>() where T : Message
            {
                types_.Add(typeof(T));
                return this;
            }

            public EventRequestBuilder WithCutoff(TimeSpan cutoff)
            {
                cutoff_ = cutoff;
                return this;
            }

            public EventRequest Build()
            {
                return new EventRequest(types_, cutoff_);
            }
        }


    }
}
