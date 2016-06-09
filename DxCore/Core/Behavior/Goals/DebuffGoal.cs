using System;
using System.Runtime.Serialization;
using DxCore.Core.Frames;

namespace DxCore.Core.Behavior.Goals
{
    // TODO (just an idea)
    [Serializable]
    [DataContract]
    public class DebuffGoal : IGoal
    {
        public ActionType ActionType => ActionType.Help;

        public TimeSpan Timeout { get; }
        public Frame Reference { get; }

        //public DebuffGoal()

        public Frame Result(GameObject entity)
        {
            throw new NotImplementedException();
        }

        public bool IsComplete(Frame reference)
        {
            throw new NotImplementedException();
        }

        public Score Utility(Frame frame)
        {
            throw new NotImplementedException();
        }
    }
}
