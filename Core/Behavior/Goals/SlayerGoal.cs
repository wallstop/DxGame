﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Frames;
using DXGame.Core.Utils;

namespace DXGame.Core.Behavior.Goals
{
    /**
        <summary>
            A goal which attempts to achieve the slaying of a target GameObject.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class SlayerGoal : AbstractGoal
    {
        private readonly GameObject target_;
        public GameObject Target => target_.Copy();
        public override ActionType ActionType => ActionType.Damage;

        public SlayerGoal(GameObject goalChaser, GameObject target, TimeSpan timeout, Frame reference)
            : base(goalChaser, timeout, reference)
        {
            Validate.IsNotNull(target.ComponentOfType<EntityPropertiesComponent>(),
                $"The target of a {nameof(SlayerGoal)} must have an {nameof(EntityPropertiesComponent)}");
            target_ = target;
        }

        public override Frame Result(GameObject entity)
        {
            /* Regardless of the entity, the result is that the target has been destroyed! */
            var targetCopy = Target;
            var targetHealth = targetCopy.ComponentOfType<EntityPropertiesComponent>().Health;
            targetHealth.CurrentValue = 0;
            return SingleElementFrame.Builder().WithGameObject(targetCopy).Build();
        }

        public override bool IsComplete(Frame reference)
        {
            var targetId = target_.Id;
            return !reference.ObjectMapping.ContainsKey(targetId) ||
                   reference.ObjectMapping[targetId].ComponentOfType<EntityPropertiesComponent>().Health.CurrentValue <=
                   0;
        }

        public override Score Utility(Frame frame)
        {
            var targetId = target_.Id;
            /* Frame doesn't have guy that should be dead? Great! That's a win in my book. */
            if (!frame.ObjectMapping.ContainsKey(targetId))
            {
                return Score.Max;
            }

            var target = frame.ObjectMapping[targetId];
            var targetHealth = target.ComponentOfType<EntityPropertiesComponent>().Health;
            return new Score(0 - targetHealth.CurrentValue);
        }
    }
}