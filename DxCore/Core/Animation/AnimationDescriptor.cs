using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Distance;

namespace DxCore.Core.Animation
{
    /*
        TODO: Bundle these & their corresponding animations as some kind of single zip file that we unpack in memory on load (+1)
    */

    [Serializable]
    [DataContract]
    public class AnimationDescriptor : JsonPersistable<AnimationDescriptor>
    {
        public static string AnimationExtension = ".adtr";

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public DxRectangle BoundingBox { get; set; } = new DxRectangle(0, 0, 50, 50);

        public override string Extension => AnimationExtension;

        [DataMember]
        public int FrameCount { get; set; }

        [DataMember]
        public AnimationFrameOffset FrameOffsets { get; set; } = AnimationFrameOffset.Empty;

        [DataMember]
        public int FramesPerSecond { get; set; } = 60;

        public override AnimationDescriptor Item => this;

        [IgnoreDataMember]
        public FrameDescriptor NewFrameDescriptor => new FrameDescriptor {BoundingBox = BoundingBox};

        [DataMember]
        public Direction Orientation { get; set; } = Direction.East;

        [DataMember]
        public double Scale { get; set; } = 1.0;

        public static AnimationDescriptor Empty()
        {
            return new AnimationDescriptor();
        }
    }
}