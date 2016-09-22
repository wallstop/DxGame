﻿using DXGame.Core.Behaviors.Goals;
using DXGame.Core.Components.Advanced.Behaviors;
using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DXGame.Core.Behaviors
{
    public interface Behavior
    {
        //? What if we push all the work onto the fitness function, where a score of 0 denotes "constraint not matched"?  Makes a 'null behavior' harder
        bool SatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments);

        Score FitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments);

        Goal ResolveGoalFor(BehaviorComponent behaver);
    }
}
