﻿using DXGame.Core.Behaviors.Goals;
using System;
using DxCore.Core.Primitives;
using DXGame.Core.Components.Advanced.Behaviors;
using NLog;

namespace DxCore.Core.Behaviors.Goals
{
    public class DoNothingGoal : Goal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DoNothingGoal() : base()
        {
            Status = GoalStatus.Successful;
        }

        public override void Update(DxGameTime gameTime, BehaviorComponent behaver)
        {
            Logger.Info($"{this.GetType().Name} updating for {nameof(behaver)}.");
            // Pass; this goal does nothing
        }
    }
}
