using DXGame.Core.Behaviors.Goals;
using DXGame.Core.Components.Advanced.Behaviors;
using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DXGame.Core.Behaviors
{
    [Serializable]
    [DataContract]
    public abstract class Behavior
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //? What if we push all the work onto the fitness function, where a score of 0 denotes "constraint not matched"?  Makes a 'null behavior' harder
        public abstract bool IsSatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments);

        public abstract Score GetFitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments);

        public abstract Goal ResolveGoalFor(BehaviorComponent behaver);
    }
}
