using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced.Triggers
{
    [DataContract]
    [Serializable]
    public class EventObserver : Component
    {
        private Func<Event, bool> Acceptance { get; }
        private Action<Event> Action { get; }

        private EventObserver(Func<Event, bool> acceptance, Action<Event> action)
        {
            Acceptance = acceptance;
            Action = action;
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
            private Func<Event, bool> eventAcceptance_;
            private Action<Event> action_;

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

            public EventObserver Build()
            {
                Validate.IsNotNull(eventAcceptance_, StringUtils.GetFormattedNullOrDefaultMessage(this, "EventAcceptance"));
                Validate.IsNotNull(action_, StringUtils.GetFormattedNullOrDefaultMessage(this, action_));
                return new EventObserver(eventAcceptance_, action_);
            }
        }
    }
}
