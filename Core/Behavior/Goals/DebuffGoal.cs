using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Frames;

namespace DXGame.Core.Behavior.Goals
{
    [Serializable]
    [DataContract]
    public class DebuffGoal : IGoal
    {
        public ActionType ActionType => ActionType.Help;

        public TimeSpan Timeout { get; }

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
