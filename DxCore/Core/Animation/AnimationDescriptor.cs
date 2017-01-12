using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Animation
{
    /*
        TODO: Bundle these & their corresponding animations as some kind of single zip file that we unpack in memory on load (+1)
    */

    [Serializable]
    [DataContract]
    public sealed class AnimationDescriptor : JsonPersistable<AnimationDescriptor>
    {
        public const int DefaultWidth = 50;
        public const int DefaultHeight = 50;
        public const float DefaultScale = 1.0f;
        public const int DefaultFps = 60;
        public static string AnimationExtension => ".adtr";

        [DataMember]
        public string Asset { get; private set; }

        public override string Extension => AnimationExtension;

        public int FrameCount => Frames.Length;

        [DataMember]
        public int FramesPerSecond { get; private set; } = DefaultFps;

        [DataMember]
        public int Height { get; private set; }

        public override AnimationDescriptor Item => this;

        public static AnimationDescriptorBuilder NewBuilder => new AnimationDescriptorBuilder();

        [DataMember]
        public Direction Orientation { get; private set; } = Direction.East;

        [DataMember]
        public float Scale { get; private set; } = DefaultScale;

        [DataMember]
        public int Width { get; private set; }

        [DataMember]
        private FrameDescriptor Fallback { get; set; }

        [DataMember]
        private FrameDescriptor[] Frames { get; set; }

        private AnimationDescriptor() {}

        private AnimationDescriptor(string asset, int width, int height, Direction orientation, float scale, int fps,
            FrameDescriptor fallback, List<FrameDescriptor> frames)
        {
            Asset = asset;
            Width = width;
            Height = height;
            Orientation = orientation;
            Scale = scale;
            Fallback = fallback;
            Frames = frames.ToArray();
        }

        public static AnimationDescriptor Empty()
        {
            return new AnimationDescriptor();
        }

        public bool OffsetForFrame(int frameNumber, out DxVector2 frameOffset, out DxVector2 drawOffset, out int width,
            out int height)
        {
            bool validFrame = frameNumber < FrameCount && 0 <= frameNumber;
            FrameDescriptor frameDescriptor = validFrame ? Frames[frameNumber] : Fallback;
            frameOffset = frameDescriptor.FrameOffset;
            drawOffset = frameDescriptor.DrawOffset;
            width = frameDescriptor.Width ?? Width;
            height = frameDescriptor.Height ?? Height;
            return validFrame;
        }

        public sealed class AnimationDescriptorBuilder : IBuilder<AnimationDescriptor>
        {
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private string Asset { get; set; }
            private FrameDescriptor Fallback { get; set; }
            private int Fps { get; set; } = DefaultFps;
            private List<FrameDescriptor> Frames { get; } = new List<FrameDescriptor>();
            private int Height { get; set; } = DefaultHeight;
            private Direction Orientation { get; set; } = Direction.East;
            private float Scale { get; set; } = DefaultScale;
            private int Width { get; set; } = DefaultWidth;

            public AnimationDescriptor Build()
            {
                Validate.Hard.IsPositive(Width, () => $"Expected {nameof(Width)} to be positive, but was {Width}");
                Validate.Hard.IsPositive(Height, () => $"Expected {nameof(Height)} to be positive, but was {Height}");
                if(Validate.Check.IsNull(Asset))
                {
                    Logger.Debug("Creating Animation with null asset");
                }
                Validate.Hard.IsPositive(Scale, () => $"Expected {nameof(Scale)} to be positive, but was {Scale}");
                if(Validate.Check.IsEmpty(Frames))
                {
                    Logger.Debug("Creating animation without any frames");
                }
                return new AnimationDescriptor(Asset, Width, Height, Orientation, Scale, Fps, Fallback, Frames);
            }

            public AnimationDescriptorBuilder WithAsset(string asset)
            {
                Asset = asset;
                return this;
            }

            public AnimationDescriptorBuilder WithDescriptor(AnimationDescriptor existingDescriptor)
            {
                Validate.Hard.IsNotNull(existingDescriptor);
                Asset = existingDescriptor.Asset;
                Fallback = existingDescriptor.Fallback;
                Fps = existingDescriptor.FramesPerSecond;
                Frames.Clear();
                foreach(FrameDescriptor frame in existingDescriptor.Frames)
                {
                    Frames.Add(frame);
                }
                Height = existingDescriptor.Height;
                Orientation = existingDescriptor.Orientation;
                Scale = existingDescriptor.Scale;
                Width = existingDescriptor.Width;
                return this;
            }

            public AnimationDescriptorBuilder WithFallback(FrameDescriptor fallback)
            {
                Fallback = fallback;
                return this;
            }

            public AnimationDescriptorBuilder WithFps(int fps)
            {
                Validate.Hard.IsPositive(fps);
                Fps = fps;
                return this;
            }

            public AnimationDescriptorBuilder WithFrame(int frameNumber, FrameDescriptor descriptor)
            {
                Validate.Hard.IsNotNull(descriptor);
                Validate.Hard.IsNotNegative(frameNumber);
                Validate.Hard.IsTrue(frameNumber <= Frames.Count);
                if(frameNumber == Frames.Count)
                {
                    Frames.Add(descriptor);
                    return this;
                }
                Frames[frameNumber] = descriptor;
                return this;
            }

            public AnimationDescriptorBuilder WithFrameCount(int frameCount)
            {
                Validate.Hard.IsNotNegative(frameCount);
                for(int i = frameCount; i < Frames.Count; ++i)
                {
                    Frames.RemoveAt(i);
                }

                for(int i = Frames.Count; i < frameCount; ++i)
                {
                    WithFrame(i, FrameDescriptor.NewFrameDescriptor);
                }
                return this;
            }

            public AnimationDescriptorBuilder WithHeight(int height)
            {
                Height = height;
                return this;
            }

            public AnimationDescriptorBuilder WithOrientation(Direction orientation)
            {
                Orientation = orientation;
                return this;
            }

            public AnimationDescriptorBuilder WithScale(float scale)
            {
                Scale = scale;
                return this;
            }

            public AnimationDescriptorBuilder WithWidth(int width)
            {
                Width = width;
                return this;
            }
        }
    }
}