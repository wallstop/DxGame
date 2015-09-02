using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Animation
{
    /*
        TODO: Bundle these & their corresponding animations as some kind of single zip file that we unpack in memory on load
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
        public int FrameCount { get; set; }

        [DataMember]
        public int FramesPerSecond { get; set; } = 60;

        [DataMember]
        public double Scale { get; set; } = 1.0;
    }
}