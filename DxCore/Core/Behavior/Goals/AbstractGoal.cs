using System;
using System.Runtime.Serialization;
using DxCore.Core.Frames;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Behavior.Goals
{
    /**
        TODO: Move interface functionality fully into abstract goal?
        <summary>
            Simple Abstract base class for goals that encapsulates a lot of functionality that is held common amongst most goals
        </summary>
    */

    [Serializable]
    [DataContract]
    public abstract class AbstractGoal : IGoal
    {
        protected readonly GameObject goalChaser_;

        protected AbstractGoal(GameObject goalChaser, TimeSpan timeout, Frame reference)
        {
            Validate.Hard.IsNotNull(goalChaser, this.GetFormattedNullOrDefaultMessage(goalChaser));
            Validate.Hard.IsNotNull(timeout, this.GetFormattedNullOrDefaultMessage("timeout"));
            Validate.Hard.IsNotNull(reference, this.GetFormattedNullOrDefaultMessage(reference));
            goalChaser_ = goalChaser;
            Reference = reference;
            Timeout = timeout;
        }

        public abstract ActionType ActionType { get; }
        public TimeSpan Timeout { get; }
        public Frame Reference { get; }
        public abstract Frame Result(GameObject entity);
        public abstract bool IsComplete(Frame reference);
        public abstract Score Utility(Frame frame);
    }
}