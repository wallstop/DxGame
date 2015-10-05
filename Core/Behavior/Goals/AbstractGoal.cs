using System;
using System.Runtime.Serialization;
using DXGame.Core.Frames;
using DXGame.Core.Utils;

namespace DXGame.Core.Behavior.Goals
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
            Validate.IsNotNull(goalChaser, StringUtils.GetFormattedNullOrDefaultMessage(this, goalChaser));
            Validate.IsNotNull(timeout, StringUtils.GetFormattedNullOrDefaultMessage(this, "timeout"));
            Validate.IsNotNull(reference, StringUtils.GetFormattedNullOrDefaultMessage(this, reference));
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