﻿using DXGame.Core.Map;
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
    public class Path
    {
        public TimeSpan Time
        {
            get;
        }

        public CommandChain Directions
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

        public Path(CommandChain directions, TimeSpan time, NavigableSurface.Node end)
        {
            Validate.IsNotNull(directions);
            Validate.IsNotNull(end);
            Time = time;
            Directions = directions;
            End = end;
        }

        public override bool Equals(object other)
        {
            var path = other as Path;
            if(ReferenceEquals(path, null))
            {
                return Objects.Equals(Time, path.Time) && Objects.Equals(End, path.End) && Objects.Equals(Directions, path.Directions);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = Objects.HashCode(Time, Directions, End);
            }
            return hash_;
        }

    }
}
