using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.DataStructures;
using DXGame.Core.Events;
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

        [DataMember] private readonly List<WeakReference<EventListener>> listeners_ = new List<WeakReference<EventListener>>();

        public override bool ShouldSerialize => false;

        public EventModel()
        {
            MessageHandler.EnableGlobalAcceptAll(HandleMessage);
        }

        private void HandleMessage(Message message)
        {
            DxGameTime currentGameTime = DxGame.Instance.CurrentTime;
            Event gameEvent = new Event(message, currentGameTime);
            events_.Add(gameEvent);
            NotifyListeners(gameEvent);
        }

        protected override void Update(DxGameTime gameTime)
        {
            Event cutoffPoint = Event.NullEventFor(gameTime.TotalGameTime - Expiry);
            events_.RemoveBelow(cutoffPoint);
            RemoveDeadListeners();
            base.Update(gameTime);
        }

        private void NotifyListeners(Event triggeredEvent)
        {
            /* Triggered */
            listeners_.ForEach(listener =>
            {
                EventListener eventListener;
                bool success = listener.TryGetTarget(out eventListener);
                if(success)
                {
                    eventListener.OnEvent(triggeredEvent);
                }
            });
        }

        private void RemoveDeadListeners()
        {
            listeners_.RemoveAll(listener =>
            {
                EventListener eventListener;
                bool stillAlive = listener.TryGetTarget(out eventListener);
                return !stillAlive;
            });
        }

        public void AttachEventListener(EventListener listener)
        {
            Validate.IsNotNullOrDefault(listener, $"Cannot attach a null {nameof(listener)} to the {typeof(EventModel)}");
            listeners_.Add(new WeakReference<EventListener>(listener));
        }

        /**

            <summary>
                Returns an unfiltered view of every kept event. 
                Modifying this list will not modify the Event Model's backing list. 
                However, modifying references will.
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
            TimeSpan cutoff = gameTime.TotalGameTime;
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
