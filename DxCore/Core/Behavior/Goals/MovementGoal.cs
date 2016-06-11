using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Frames;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Behavior.Goals
{
    /**
        <summary>
            Attempts to achieve the movement of one entity to a place
        </summary>
    */

    [Serializable]
    [DataContract]
    public class MovementGoal : AbstractGoal
    {
        /* How far away from the goal we are to consider ourself to have "reached the goal" */
        private static readonly int GOAL_TOLERANCE = 3;
        public override ActionType ActionType => ActionType.Movement;

        [DataMember]
        public DxVector2 Target { get; }

        public MovementGoal(GameObject goalChaser, TimeSpan timeout, Frame reference, DxVector2 target)
            : base(goalChaser, timeout, reference)
        {
            var mapModel = DxGame.Instance.Model<MapModel>();
            Validate.IsTrue(mapModel.MapBounds.Contains(target),
                $"{target} was not found to be within {mapModel.MapBounds}");
            /* We need a position if we're going to move somewhere (and probably a physics) */
            var positionalComponent = goalChaser.ComponentOfType<PositionalComponent>();
            Validate.IsNotNull(positionalComponent, this.GetFormattedNullOrDefaultMessage(positionalComponent));
            Target = target;
        }

        public override Frame Result(GameObject entity)
        {
            if(!Objects.Equals(goalChaser_, entity))
            {
                return Frame.EmptyFrame;
            }

            var copiedGameObject = goalChaser_.Copy();
            /* Result is whatever the entity is now, but with it's position at the target */
            var position = copiedGameObject.ComponentOfType<PositionalComponent>();
            position.Position = Target;
            return SingleElementFrame.Builder().WithGameObject(copiedGameObject).Build();
        }

        public override bool IsComplete(Frame reference)
        {
            /* If our goal chaser is not part of the frame, give up, go home */
            if(!reference.ObjectMapping.ContainsKey(goalChaser_.Id))
            {
                return true;
            }

            var currentPosition =
                reference.ObjectMapping[goalChaser_.Id].ComponentOfType<PositionalComponent>().Position;
            return Math.Abs((currentPosition - Target).Magnitude) < GOAL_TOLERANCE;
        }

        public override Score Utility(Frame frame)
        {
            var entityId = goalChaser_.Id;
            if(!frame.ObjectMapping.ContainsKey(entityId))
            {
                /* Entity isn't here? That's the worst. */
                return Score.Min;
            }

            var currentEntityPosition = frame.ObjectMapping[entityId].ComponentOfType<PositionalComponent>().Position;
            /* Lower score is better, therefore closer is better */
            return new Score((Target - currentEntityPosition).Magnitude);
        }
    }
}