using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Triggers
{
    // TODO: This is kind of annoying to use. Can we... do something better?
    [DataContract]
    [Serializable]
    [ProtoContract]
    public class EventObserver : Component
    {
        [ProtoMember(1)]
        [DataMember]
        private Func<Event, bool> Acceptance { get; }

        [ProtoMember(2)]
        [DataMember]
        private Action<Event> Action { get; }

        [ProtoMember(3)]
        [DataMember]
        public EventListener Listener { get; }

        private EventObserver(Func<Event, bool> acceptance, Action<Event> action)
        {
            Acceptance = acceptance;
            Action = action;
            Listener = new EventListener(HandleEvent);
            DxGame.Instance.Model<EventModel>().AttachEventListener(Listener);
        }

        public void HandleEvent(Event gameEvent)
        {
            bool ofInterest = Acceptance(gameEvent);
            if(ofInterest)
            {
                Action(gameEvent);
            }
        }

        public static Builder EventObserverBuilder()
        {
            return new Builder();
        }

        public class Builder : IBuilder<EventObserver>
        {
            private Action<Event> action_;
            private Func<Event, bool> eventAcceptance_;

            public EventObserver Build()
            {
                Validate.IsNotNull(eventAcceptance_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "EventAcceptance"));
                Validate.IsNotNull(action_, StringUtils.GetFormattedNullOrDefaultMessage(this, action_));
                return new EventObserver(eventAcceptance_, action_);
            }

            public Builder WithAction(Action<Event> action)
            {
                action_ = action;
                return this;
            }

            public Builder WithAcceptance(Func<Event, bool> acceptanceFunction)
            {
                eventAcceptance_ = acceptanceFunction;
                return this;
            }
        }
    }
}