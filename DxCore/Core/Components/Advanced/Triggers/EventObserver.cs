using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Events;
using DxCore.Core.Models;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Triggers
{
    // TODO: This is kind of annoying to use. Can we... do something better?
    [DataContract]
    [Serializable]
    public class EventObserver : Component
    {
        [DataMember]
        private Func<Event, bool> Acceptance { get; }

        [DataMember]
        private Action<Event> Action { get; }

        [DataMember]
        public EventListener Listener { get; }

        private EventObserver(Func<Event, bool> acceptance, Action<Event> action)
        {
            Acceptance = acceptance;
            Action = action;
            Listener = new EventListener(HandleEvent);
        }

        public override void Initialize()
        {
            if(!Initialized)
            {
                DxGame.Instance.Model<EventModel>().AttachEventListener(Listener);
            }
            base.Initialize();
        }

        public void HandleEvent(Event gameEvent)
        {
            bool ofInterest = Acceptance(gameEvent);
            if(ofInterest)
            {
                Action(gameEvent);
            }
        }

        public void Uninitialize()
        {
            // TODO
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
                Validate.Hard.IsNotNull(eventAcceptance_, this.GetFormattedNullOrDefaultMessage("EventAcceptance"));
                Validate.Hard.IsNotNull(action_, this.GetFormattedNullOrDefaultMessage(action_));
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