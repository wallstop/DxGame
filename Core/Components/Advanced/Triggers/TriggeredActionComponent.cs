using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

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
    [ProtoContract]
    public class TriggeredActionComponent<T> : Component
    {
        [ProtoMember(1)] [DataMember] private readonly Action<T> action_;

        /* Determines whether or not this should stop based off of (original time invoked) (current GameTime) */
        [ProtoMember(2)] [DataMember] private readonly Func<TimeSpan, DxGameTime, bool> endTrigger_;

        /* Triggered once the Action has ended */
        [ProtoMember(3)] [DataMember] private readonly Action<T> finalAction_;

        [ProtoMember(4)] [DataMember] private readonly T source_;

        [ProtoMember(5)] [DataMember] private readonly Func<DxGameTime, int> tickTrigger_;

        [ProtoMember(6)]
        [DataMember]
        public TimeSpan Initialized { get; }

        public TriggeredActionComponent(Func<TimeSpan, DxGameTime, bool> endTrigger, Func<DxGameTime, int> tickTrigger,
            T source, Action<T> action) : this(endTrigger, tickTrigger, source, action, type => { }) {}

        public TriggeredActionComponent(Func<TimeSpan, DxGameTime, bool> endTrigger, Func<DxGameTime, int> tickTrigger,
            T source, Action<T> action, Action<T> finalAction)
        {
            Validate.IsNotNullOrDefault(endTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(endTrigger)));
            Validate.IsNotNullOrDefault(tickTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(tickTrigger)));
            Validate.IsNotNull(action, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(action)));
            Validate.IsNotNullOrDefault(finalAction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(finalAction)));

            endTrigger_ = endTrigger;
            tickTrigger_ = tickTrigger;
            source_ = source;
            action_ = action;
            finalAction_ = finalAction;
            Initialized = DxGame.Instance.CurrentTime.TotalGameTime;
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* 
                We don't want to gaurantee only one trigger per update - there may be multiple. 
                Check how many, and invoke the trigger that many times 
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
                finalAction_.Invoke(source_);
                Remove();
            }
        }
    }

    /**
        TODO: Refactor this with the above... somehow
    */

    [DataContract]
    [Serializable]
    [ProtoContract]
    public class TriggeredActionComponent : Component
    {
        [ProtoMember(1)] [DataMember] private readonly Action action_;

        /* Determines whether or not this should stop based off of (original time invoked) (current GameTime) */

        [ProtoMember(2)] [DataMember] private readonly Func<TimeSpan, DxGameTime, bool> endTrigger_;

        /* Triggered once the Action has ended */

        [ProtoMember(3)] [DataMember] private readonly Action finalAction_;

        [ProtoMember(4)] [DataMember] private readonly Func<DxGameTime, int> tickTrigger_;

        [ProtoMember(5)]
        [DataMember]
        public TimeSpan Initialized { get; }

        public TriggeredActionComponent(Func<TimeSpan, DxGameTime, bool> endTrigger, Func<DxGameTime, int> tickTrigger,
            Action action) : this(endTrigger, tickTrigger, action, () => { }) {}

        public TriggeredActionComponent(Func<TimeSpan, DxGameTime, bool> endTrigger, Func<DxGameTime, int> tickTrigger,
            Action action, Action finalAction)
        {
            Validate.IsNotNullOrDefault(endTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(endTrigger)));
            Validate.IsNotNullOrDefault(tickTrigger,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(tickTrigger)));
            Validate.IsNotNull(action, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(action)));
            Validate.IsNotNullOrDefault(finalAction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(finalAction)));

            endTrigger_ = endTrigger;
            tickTrigger_ = tickTrigger;
            action_ = action;
            finalAction_ = finalAction;
            Initialized = DxGame.Instance.CurrentTime.TotalGameTime;
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* 
                We don't want to gaurantee only one trigger per update - there may be multiple. 
                Check how many, and invoke the trigger that many times 
            */
            int numTriggers = tickTrigger_.Invoke(gameTime);
            for(int i = 0; i < numTriggers; ++i)
            {
                action_.Invoke();
            }

            /* 
                And finally, see if we're done. We want to check after we (potentially) trigger, 
                as we may have something like single-frame triggers 
                that rely on being activated that one time (and one time only) 
            */
            bool isFinished = endTrigger_.Invoke(Initialized, gameTime);
            if(isFinished)
            {
                finalAction_.Invoke();
                Remove();
            }
        }
    }
}