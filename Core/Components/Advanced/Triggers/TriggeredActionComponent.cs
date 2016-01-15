using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Triggers
{
    /**
        <summary>
            Generically modifies some T (such as EntityProperties) based on triggers being fulfilled 
            - one trigger to control ticks, one trigger to control when to stop
        </summary>
    */

    [DataContract]
    [Serializable]
    public class TriggeredActionComponent<T> : Component
    {
        [DataMember] private readonly Action<T> action_;

        /* Determines whether or not this should stop based off of (original time invoked) (current GameTime) */
        [DataMember] private readonly Func<TimeSpan, DxGameTime, bool> endTrigger_;

        [DataMember] private readonly T source_;

        [DataMember] private readonly Func<DxGameTime, int> tickTrigger_;

        [DataMember]
        public TimeSpan Initialized { get; }

        public TriggeredActionComponent(Func<TimeSpan, DxGameTime, bool> endTrigger, Func<DxGameTime, int> tickTrigger,
            T source, Action<T> action)
        {
            Validate.IsNotNullOrDefault(endTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(endTrigger)));
            Validate.IsNotNullOrDefault(tickTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(tickTrigger)));
            Validate.IsNotNull(action, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(action)));

            endTrigger_ = endTrigger;
            tickTrigger_ = tickTrigger;
            source_ = source;
            action_ = action;
            Initialized = DxGame.Instance.CurrentTime.TotalGameTime;
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* 
                We don't want to gaurantee only one trigger per update - there may be multiple. 
                Check how many, and trigger that many times 
            */
            int numTriggers = tickTrigger_.Invoke(gameTime);
            for(int i = 0; i < numTriggers; ++i)
            {
                action_.Invoke(source_);
            }

            /* 
                And finally, see if we're done. We want to check after we (potentially) trigger, 
                as we may have something like single-frame triggers 
                that rely on being activated that one time (and one time only) 
            */
            bool isFinished = endTrigger_.Invoke(Initialized, gameTime);
            if(isFinished)
            {
                Dispose();
            }
        }
    }
}