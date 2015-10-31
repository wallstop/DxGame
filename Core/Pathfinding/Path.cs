using DXGame.Core.Map;
using DXGame.Core.Utils;
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
    public class Path : IComparable<Path>
    {
        public TimeSpan Time
        {
            get;
        }

        public CommandChain Directions
        {
            get;
        }

        public NavigableSurface.Node Start
        {
            get;
        }

        public NavigableSurface.Node End
        {
            get;
        }

        [NonSerialized]
        [IgnoreDataMember]
        private int hash_;

        public Path(CommandChain directions, TimeSpan time, NavigableSurface.Node start, NavigableSurface.Node end)
        {
            Validate.IsNotNull(directions);
            Validate.IsNotNull(end);
            Time = time;
            Directions = directions;
            End = end;
            Start = start;
        }

        public override bool Equals(object other)
        {
            var path = other as Path;
            if(!ReferenceEquals(path, null))
            {
                return Objects.Equals(End, path.End) && Objects.Equals(Directions, path.Directions);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = Objects.HashCode(Directions, End);
            }
            return hash_;
        }

        public int CompareTo(Path other)
        {
            if(ReferenceEquals(other, null))
            {
                return 1;
            }
            int endCompare = End.CompareTo(other.End);
            if(endCompare != 0)
            {
                return endCompare;
            }
            int commandChainCompare = Directions.CompareTo(other.Directions);
            if(commandChainCompare != 0)
            {
                return commandChainCompare;
            }
            int timeCompare = Time.CompareTo(other.Time);
            return timeCompare;
        }
    }
}
