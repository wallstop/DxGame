using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Animation
{
    /**
        <summary>
            DAO for storing frame-specific offsets for animations
        </summary>
    */
    [Serializable]
    [DataContract]
    public class AnimationFrameOffset
    {
        private static readonly Lazy<AnimationFrameOffset> SINGLETON = new Lazy<AnimationFrameOffset>(() => new AnimationFrameOffset());

        public static AnimationFrameOffset Instance => SINGLETON.Value;

        [DataMember]
        private Dictionary<int, FrameDescriptor> OffsetsByFrame { get; set; }

        /* Fallback in case no frame-specific information is found */
        [DataMember]
        private FrameDescriptor Fallback { get; set; }

        private AnimationFrameOffset()
        {
            OffsetsByFrame = new Dictionary<int, FrameDescriptor>();
        }

        public AnimationFrameOffset(Dictionary<int, FrameDescriptor> offsetsByFrame) : this(offsetsByFrame, FrameDescriptor.Instance) {}

        public AnimationFrameOffset(Dictionary<int, FrameDescriptor> offsetsByFrame, FrameDescriptor fallback)
        {
            Validate.IsNotNull(offsetsByFrame,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(offsetsByFrame)));
            OffsetsByFrame = offsetsByFrame;
            Fallback = fallback;
        }

        public void OffsetForFrame(int frameNumber, out DxVector2 frameOffset, out DxVector2 drawOffset, out DxRectangle boundingBox)
        {
            FrameDescriptor frameDescriptor;
            if(OffsetsByFrame.TryGetValue(frameNumber, out frameDescriptor))
            {
                frameOffset = frameDescriptor.FrameOffset;
                drawOffset = frameDescriptor.DrawOffset;
                boundingBox = frameDescriptor.BoundingBox;
                return;
            }
            // TODO: Log failure?
            frameOffset = Fallback.FrameOffset;
            drawOffset = Fallback.DrawOffset;
            boundingBox = Fallback.BoundingBox;
            return;
        }
    }
}
