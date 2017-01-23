using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Animation
{
    /**
        <summary>
            Simple DAO encapsulating data for frame-specific information
        </summary>
    */

    [Serializable]
    [DataContract]
    public struct FrameDescriptor
    {
        public static FrameDescriptor NewFrameDescriptor { get; } = new FrameDescriptor();

        public FrameDescriptor(FrameDescriptor other)
        {
            FrameOffset = other.FrameOffset;
            DrawOffset = other.DrawOffset;
            Width = other.Width;
            Height = other.Height;
        }

        /* Displacement vector for the actual start of the image within a frame */

        [DataMember]
        public DxVector2 FrameOffset { get; set; }

        /* Displacement vector to draw the image at (so we can deal with larger-than-bounding-box images) */

        [DataMember]
        public DxVector2 DrawOffset { get; set; }

        [DataMember]
        public int? Width { get; set; }

        [DataMember]
        public int? Height { get; set; }
    }
}