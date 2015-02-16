using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Frames;
using DXGame.Core.Utils;

namespace DXGame.Core.Models
{
    public class FrameModel
    {
        public TimeSpan FrameRetention { get; protected set; }
        protected List<GameTimeFrame> Frames { get; set; }

        public FrameModel()
        {
            Frames = new List<GameTimeFrame>();
        }

        public FrameModel WithFrameRetention(TimeSpan timespan)
        {
            GenericUtils.CheckNullOrDefault(timespan,
                "Cannot create a FrameModel without a null/default FrameRetention policy");
            FrameRetention = timespan;
            return this;
        }

        public void AttachFrame(GameTimeFrame frame)
        {
            Frames.Add(frame);
        }

        public GameTimeFrame LatestFrame()
        {
            return Frames.Last();
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
            foreach (GameTimeFrame frame in Frames)
            {
            }
        }
    }
}