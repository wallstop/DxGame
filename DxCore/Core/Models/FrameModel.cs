using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Frames;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Models
{
    public class FrameModel : Model
    {
        public TimeSpan FrameRetention { get; protected set; }
        protected List<Frame> Frames { get; set; }

        public FrameModel()
        {
            Frames = new List<Frame>();
        }

        public FrameModel WithFrameRetention(TimeSpan timespan)
        {
            Validate.IsNotNullOrDefault(timespan, this.GetFormattedNullOrDefaultMessage(timespan));
            FrameRetention = timespan;
            return this;
        }

        public override bool ShouldSerialize => false;

        public void AttachFrame(Frame frame)
        {
            Validate.IsTrue(!Frames.Contains(frame),
                $"Cannot attach frame {frame} to {GetType()}. This frame already exists in {Frames}");
            Frames.Add(frame);
        }

        public Frame LatestFrame()
        {
            return Frames.LastOrDefault();
        }

        public bool HasFrame()
        {
            return Frames.Any();
        }

        protected void CullFrames()
        {
            /*
                Remove all frames that fall outside of our FrameRetention policy.
                This will be done either by taking (GameTime of our latest frame - FrameRetention),
                or by marking each Frame with a timestamp of when we receive it, and culling all of those
                with timestamp >= FrameRetention.

                Need to make sure we don't cull things if FrameRetention = 0
            */
            foreach(Frame frame in Frames)
            {
                // TODO
            }
        }
    }
}