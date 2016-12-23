using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Animation
{
    /**
        <summary>
            DAO for storing frame-specific offsets for animations
        </summary>
    */

    [Serializable]
    [DataContract]
    public sealed class AnimationFrameOffset
    {
        public static AnimationFrameOffset Empty { get; } = new AnimationFrameOffset();

        [IgnoreDataMember]
        public ReadOnlyDictionary<int, FrameDescriptor> Offsets
            => new ReadOnlyDictionary<int, FrameDescriptor>(OffsetsByFrame);

        /* Fallback in case no frame-specific information is found */

        [DataMember]
        private FrameDescriptor Fallback { get; set; }

        [DataMember]
        private Dictionary<int, FrameDescriptor> OffsetsByFrame { get; set; }

        private AnimationFrameOffset() : this(new Dictionary<int, FrameDescriptor>()) {}

        private AnimationFrameOffset(Dictionary<int, FrameDescriptor> offsetsByFrame)
            : this(offsetsByFrame, FrameDescriptor.NewFrameDescriptor) {}

        private AnimationFrameOffset(Dictionary<int, FrameDescriptor> offsetsByFrame, FrameDescriptor fallback)
        {
            Validate.Hard.IsNotNull(offsetsByFrame, this.GetFormattedNullOrDefaultMessage(nameof(offsetsByFrame)));
            OffsetsByFrame = offsetsByFrame;
            Fallback = fallback;
        }

        /**
            <summary>
                Fills out the frame offset, draw offset, and bounding box of the frame, returning true if these value were found, false if they were fallbacks.
            </summary>
        */

        public bool OffsetForFrame(int frameNumber, out DxVector2 frameOffset, out DxVector2 drawOffset,
            out DxRectangle boundingBox)
        {
            FrameDescriptor frameDescriptor;
            bool found = OffsetForFrame(frameNumber, out frameDescriptor);
            frameOffset = frameDescriptor.FrameOffset;
            drawOffset = frameDescriptor.DrawOffset;
            boundingBox = frameDescriptor.BoundingBox;
            return found;
        }

        public bool OffsetForFrame(int frameNumber, out FrameDescriptor frameDescriptor)
        {
            if(OffsetsByFrame.TryGetValue(frameNumber, out frameDescriptor))
            {
                return true;
            }
            frameDescriptor = Fallback;
            return false;
        }

        public sealed class AnimationFrameOffsetBuilder : IBuilder<AnimationFrameOffset>
        {
            private readonly Dictionary<int, FrameDescriptor> offsetsByFrame_ = new Dictionary<int, FrameDescriptor>();
            private FrameDescriptor fallback_ = FrameDescriptor.NewFrameDescriptor;

            public AnimationFrameOffset Build()
            {
                return new AnimationFrameOffset(offsetsByFrame_, fallback_);
            }

            public AnimationFrameOffsetBuilder WithAnimationFrameOffset(AnimationFrameOffset offsets)
            {
                foreach(KeyValuePair<int, FrameDescriptor> entry in offsets.OffsetsByFrame)
                {
                    offsetsByFrame_[entry.Key] = entry.Value;
                }
                return this;
            }

            public AnimationFrameOffsetBuilder WithBoundingBox(DxRectangle bounds)
            {
                WithHeight(bounds.Height);
                WithWidth(bounds.Width);
                return this;
            }

            public AnimationFrameOffsetBuilder WithFallback(FrameDescriptor fallback)
            {
                fallback_ = fallback;
                return this;
            }

            public AnimationFrameOffsetBuilder WithFrameOffset(int frameNumber, FrameDescriptor descriptor)
            {
                Validate.Hard.IsPositive(frameNumber);
                offsetsByFrame_[frameNumber] = descriptor;
                return this;
            }

            public AnimationFrameOffsetBuilder WithHeight(float height)
            {
                foreach(KeyValuePair<int, FrameDescriptor> frameAndDescriptor in offsetsByFrame_.ToArray())
                {
                    DxRectangle modifiedBoundingBox = frameAndDescriptor.Value.BoundingBox;
                    modifiedBoundingBox.Height = height;

                    FrameDescriptor updatedFrameDescriptor = new FrameDescriptor(frameAndDescriptor.Value)
                    {
                        BoundingBox = modifiedBoundingBox
                    };

                    offsetsByFrame_[frameAndDescriptor.Key] = updatedFrameDescriptor;
                }
                return this;
            }

            public AnimationFrameOffsetBuilder WithoutFrameOffset(int frameNumber)
            {
                offsetsByFrame_.Remove(frameNumber);
                return this;
            }

            public AnimationFrameOffsetBuilder WithWidth(float width)
            {
                // TODO: Refactor
                foreach(KeyValuePair<int, FrameDescriptor> frameAndDescriptor in offsetsByFrame_.ToArray())
                {
                    DxRectangle modifiedBoundingBox = frameAndDescriptor.Value.BoundingBox;
                    modifiedBoundingBox.Width = width;

                    FrameDescriptor updatedFrameDescriptor = new FrameDescriptor(frameAndDescriptor.Value)
                    {
                        BoundingBox = modifiedBoundingBox
                    };

                    offsetsByFrame_[frameAndDescriptor.Key] = updatedFrameDescriptor;
                }
                return this;
            }
        }
    }
}