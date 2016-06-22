using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Pathfinding
{
    /**
        <summary>
            Simple data holder for useful pathfinding related info
        </summary>
    */
    [Serializable]
    [DataContract]
    public class PathfindingResult
    {
        public LinkedList<ImmutablePair<TimeSpan, CommandChain>> Path { get; }

        public LinkedList<DxVector2> WayPoints { get; }

        public bool Successful { get; }

        public PathfindingResult(LinkedList<ImmutablePair<TimeSpan, CommandChain>> path, LinkedList<DxVector2> waypoints, bool success = false)
        {
            Validate.Hard.IsNotNull(path, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(path)));
            Validate.Hard.IsNotNull(waypoints, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(waypoints)));
            Path = path;
            WayPoints = waypoints;
            Successful = success;
        }

        public static PathfindingResult EmptyResult => new PathfindingResult(new LinkedList<ImmutablePair<TimeSpan, CommandChain>>(), new LinkedList<DxVector2>(), false);
    }
}
