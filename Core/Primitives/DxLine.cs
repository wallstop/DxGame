using System;
using System.Linq;
using System.Runtime.Serialization;

namespace DXGame.Core.Primitives
{
    [Serializable]
    [DataContract]
    public struct DxLine
    {
        [DataMember]
        public DxVector2 Start { get; set; }

        [DataMember]
        public DxVector2 End { get; set; }

        public DxVector2 Vector => new DxVector2(End.X - Start.X, End.Y - Start.Y);

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
            var firstOrientation = DxVector2.Orientation(Start, End, other.Start);
            var secondOrientation = DxVector2.Orientation(Start, End, other.End);
            var thirdOrientation = DxVector2.Orientation(other.Start, other.End, Start);
            var fourthOrientation = DxVector2.Orientation(other.Start, other.End, End);
            return (firstOrientation != secondOrientation) && (thirdOrientation != fourthOrientation);
        }

        public bool Intersects(DxRectangle rectangle)
        {
            var localCopy = this;
            return rectangle.Lines.Any(line => localCopy.Intersects(line));
        }
    }
}