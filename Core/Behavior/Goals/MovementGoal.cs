using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Frames;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Behavior.Goals
{
    [Serializable]
    [DataContract]
    public class MovementGoal : AbstractGoal
    {
        public override ActionType ActionType => ActionType.Movement;

        [DataMember]
        public DxVector2 Target { get; }

        public override Frame Result(GameObject entity)
        {
            throw new NotImplementedException();
        }

        public override bool IsComplete(Frame reference)
        {
            throw new NotImplementedException();
        }

        public override Score Utility(Frame frame)
        {
            throw new NotImplementedException();
        }

        public MovementGoal(GameObject goalChaser, TimeSpan timeout, Frame reference, DxVector2 target)
            : base(goalChaser, timeout, reference)
        {
            var mapModel = DxGame.Instance.Model<MapModel>();
            Validate.IsTrue(mapModel.MapBounds.Contains(target), $"{target} was not found to be within {mapModel.MapBounds}");
            var physicsComponent = goalChaser.ComponentOfType<PhysicsComponent>();
            Validate.IsNotNull(physicsComponent, StringUtils.GetFormattedNullOrDefaultMessage(this, physicsComponent));
            Target = target;
        }
    }
}
