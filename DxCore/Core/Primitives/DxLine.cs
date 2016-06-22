﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Utils;

namespace DxCore.Core.Primitives
{
    [Serializable]
    [DataContract]
    public struct DxLine : IEquatable<DxLine>
    {
        [DataMember]
        public DxVector2 Start { get; set; }

        [DataMember]
        public DxVector2 End { get; set; }

        public DxVector2 Vector => new DxVector2(End.X - Start.X, End.Y - Start.Y);

        /* Careful with vertical lines */
        public float Slope => (End.Y - Start.Y) / (End.X - Start.X);

        public DxLine(DxVector2 start, DxVector2 end)
        {
            Start = start;
            End = end;
        }

        public DxLine(float x1, float y1, float x2, float y2)
        {
            Start = new DxVector2(x1, y1);
            End = new DxVector2(x2, y2);
        }

        /* Borrowed heavily from http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/ */

        public bool Intersects(DxLine other)
        {
            Orientation firstOrientation = DxVector2.Orientation(Start, End, other.Start);
            Orientation secondOrientation = DxVector2.Orientation(Start, End, other.End);
            Orientation thirdOrientation = DxVector2.Orientation(other.Start, other.End, Start);
            Orientation fourthOrientation = DxVector2.Orientation(other.Start, other.End, End);
            return (firstOrientation != secondOrientation) && (thirdOrientation != fourthOrientation);
        }

        public bool Intersects(DxRectangle rectangle)
        {
            var localCopy = this;
            return rectangle.Lines.Any(line => localCopy.Intersects(line));
        }

        public override bool Equals(object other)
        {
            if(other is DxLine)
            {
                return Equals((DxLine) other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Start, End);
        }

        public override string ToString()
        {
            return $"{{ Start:{Start}, End:{End} }}";
        }

        public bool Equals(DxLine other)
        {
            return this == other;
        }

        public static bool operator ==(DxLine lhs, DxLine rhs)
        {
            return lhs.Start == rhs.Start && lhs.End == rhs.End;
        }

        public static bool operator !=(DxLine lhs, DxLine rhs)
        {
            return !(lhs == rhs);
        }
    }
}