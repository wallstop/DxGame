using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Pathfinding
{
    [Serializable]
    [DataContract]
    public class PathfindingResult
    {
        public LinkedList<ImmutablePair<TimeSpan, Commandment[]>> Path { get; }

        public LinkedList<DxVector2> WayPoints { get; }

        public PathfindingResult(LinkedList<ImmutablePair<TimeSpan, Commandment[]>> path, LinkedList<DxVector2> waypoints)
        {
            Validate.IsNotNull(path, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(path)));
            Validate.IsNotNull(waypoints, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(waypoints)));
            Path = path;
            WayPoints = waypoints;
        }
    }
}
