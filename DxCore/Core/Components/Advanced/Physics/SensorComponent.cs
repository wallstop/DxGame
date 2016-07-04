using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using FarseerPhysics.Dynamics;
using NLog;

namespace DxCore.Core.Components.Advanced.Physics
{
    // TODO: Should these be components or simply objects?

        // TODO: Remove
    [Serializable]
    [DataContract]
    public sealed class SensorComponent : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // TODO: Hide access to these better (immutable getters)
        [DataMember]
        public List<BeforeCollisionEventHandler> BeforeCollisionEventHandlers { get; private set; }

        [DataMember]
        public List<OnCollisionEventHandler> OnCollisionEventHandlers { get; private set; }

        [DataMember]
        public List<AfterCollisionEventHandler> AfterCollisionEventHandlers { get; private set; }

        private SensorComponent(List<BeforeCollisionEventHandler> beforeCollisionEventHandlers,
            List<OnCollisionEventHandler> onCollisionEventHandlers,
            List<AfterCollisionEventHandler> afterCollisionEventHandlers)
        {
            BeforeCollisionEventHandlers = beforeCollisionEventHandlers;
            OnCollisionEventHandlers = onCollisionEventHandlers;
            AfterCollisionEventHandlers = afterCollisionEventHandlers;
        }

        public override void OnAttach()
        {
            PhysicsComponent attachedTo = Parent.ComponentOfType<PhysicsComponent>();
            if(ReferenceEquals(attachedTo, null))
            {
                Logger.Debug("Attached a {0} to an object wihout any {1}. This is likely a bug.",
                    typeof(SensorComponent), typeof(PhysicsComponent));
            }
            else
            {
                attachedTo.Attach(this);
            }
        }

        protected override void CustomOnDetach()
        {
            PhysicsComponent attachedTo = Parent?.ComponentOfType<PhysicsComponent>();
            if(ReferenceEquals(attachedTo, null))
            {
                Logger.Debug("Unable to remove {0}, can't find attached {1}", this, typeof(PhysicsComponent));
            }
            else
            {
                attachedTo.Detach(this);
            }
        }

        public static SensorComponentBuilder Builder()
        {
            return new SensorComponentBuilder();
        }

        public sealed class SensorComponentBuilder : IBuilder<SensorComponent>
        {
            private readonly HashSet<BeforeCollisionEventHandler> before_ = new HashSet<BeforeCollisionEventHandler>();
            private readonly HashSet<AfterCollisionEventHandler> after_ = new HashSet<AfterCollisionEventHandler>();
            private readonly HashSet<OnCollisionEventHandler> on_ = new HashSet<OnCollisionEventHandler>();

            public SensorComponentBuilder WithOnCollisionHandler(OnCollisionEventHandler onCollisionHandler)
            {
                Validate.Hard.IsNotNullOrDefault(onCollisionHandler);
                bool newHandler = on_.Add(onCollisionHandler);
                if(!newHandler)
                {
                    Logger.Debug("Ignoring duplicate {0} handler for {1}", nameof(onCollisionHandler),
                        typeof(SensorComponent));
                }
                return this;
            }

            public SensorComponentBuilder WithAfterCollisionHandler(AfterCollisionEventHandler afterCollisionHandler)
            {
                Validate.Hard.IsNotNullOrDefault(afterCollisionHandler);
                bool newHandler = after_.Add(afterCollisionHandler);
                if(!newHandler)
                {
                    Logger.Debug("Ignoring duplicate {0} handler for {1}", nameof(afterCollisionHandler),
                        typeof(SensorComponent));
                }
                return this;
            }

            public SensorComponentBuilder WithBeforeCollisionHandler(BeforeCollisionEventHandler beforeCollisionHandler)
            {
                Validate.Hard.IsNotNullOrDefault(beforeCollisionHandler);
                bool newHandler = before_.Add(beforeCollisionHandler);
                if(!newHandler)
                {
                    Logger.Debug("Ignoring duplicate {0} handler for {1}", nameof(beforeCollisionHandler),
                        typeof(SensorComponent));
                }
                return this;
            }

            public SensorComponent Build()
            {
                if(!before_.Any() || !on_.Any() || !after_.Any())
                {
                    Logger.Debug("Creating a {0} without any event handlers, is this intentional?",
                        typeof(SensorComponent));
                }
                return new SensorComponent(before_.ToList(), on_.ToList(), after_.ToList());
            }
        }
    }
}
