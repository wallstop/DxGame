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
        public override string Extension => AnimationExtension;
        public override AnimationDescriptor Item => this;

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public DxRectangle BoundingBox { get; set; } = new DxRectangle(0, 0, 50, 50);

        [DataMember]
        public AnimationFrameOffset FrameOffsets { get; set; } = AnimationFrameOffset.Instance;

        [DataMember]
        public int FrameCount { get; set; }

        [DataMember]
        public int FramesPerSecond { get; set; } = 60;

        [DataMember]
        public double Scale { get; set; } = 1.0;

        [DataMember]
        public Direction Orientation { get; set; } = Direction.East;
    }
}