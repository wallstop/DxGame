using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Primitives
{
    [Serializable]
    [DataContract]
    public struct DxLineSegment : IEquatable<DxLineSegment>
    {
        [DataMember]
        public DxVector2 Start { get; set; }

        [DataMember]
        public DxVector2 End { get; set; }

        public DxVector2 Vector => new DxVector2(End.X - Start.X, End.Y - Start.Y);

        public DxLineSegment Reverse => new DxLineSegment(End, Start);

        /* Careful with vertical lines */
        public float Slope => (End.Y - Start.Y) / (End.X - Start.X);

        public DxLineSegment(DxVector2 start, DxVector2 end)
        {
            Start = start;
            End = end;
        }

        /**
            <summary>
                Scales the LineSegment as if it were anchored at its current endpoints and the z axis is being scaled by the provided scalar.
            </summary>
            <notes> 
                scalars larger than one will grow the vector, fractional will shrink. Negative scalars are not allowed.
            </notes>

            {(1, 1), (3,3)} scaledInPlace by 0.5 -> {(1.5,1.5), (2.5,2.5)}
        */

        public DxLineSegment ScaleInPlace(float scalar)
        {
            Validate.Hard.IsNotNegative(scalar,
                () => $"Cannot shrink a {typeof(DxLineSegment)} with a scalar of {scalar}");
            DxVector2 currentOffset = End - Start;
            float offsetMagnitude = currentOffset.Magnitude;
            float newMagnitude = offsetMagnitude * scalar;
            float magnitudeDifference = offsetMagnitude - newMagnitude;
            DxVector2 newOffset = currentOffset.UnitVector * (magnitudeDifference / 2);
            return new DxLineSegment(Start + newOffset, End - newOffset);
        }

        public DxLineSegment(float x1, float y1, float x2, float y2)
        {
            Start = new DxVector2(x1, y1);
            End = new DxVector2(x2, y2);
        }

        /* Borrowed heavily from http://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/ */

        public bool Intersects(DxLineSegment other)
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
            if(other is DxLineSegment)
            {
                return Equals((DxLineSegment) other);
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

        public bool Equals(DxLineSegment other)
        {
            return this == other;
        }

        public static bool operator ==(DxLineSegment lhs, DxLineSegment rhs)
        {
            return lhs.Start == rhs.Start && lhs.End == rhs.End;
        }

        public static bool operator !=(DxLineSegment lhs, DxLineSegment rhs)
        {
            return !(lhs == rhs);
        }
    }
}