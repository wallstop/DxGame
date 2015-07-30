﻿using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Frames;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public class FrameModel : Model
    {
        public TimeSpan FrameRetention { get; protected set; }
        protected List<GameTimeFrame> Frames { get; set; }

        public FrameModel(DxGame game)
            : base(game)
        {
            Frames = new List<GameTimeFrame>();
        }

        public FrameModel WithFrameRetention(TimeSpan timespan)
        {
            Validate.IsNotNullOrDefault(timespan, StringUtils.GetFormattedNullOrDefaultMessage(this, timespan));
            FrameRetention = timespan;
            return this;
        }

        public void AttachFrame(GameTimeFrame frame)
        {
            Validate.IsTrue(!Frames.Contains(frame),
                $"Cannot attach frame {frame} to {GetType()}. This frame already exists in {Frames}");
            Frames.Add(frame);
        }

        public GameTimeFrame LatestFrame()
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
            foreach (GameTimeFrame frame in Frames)
            {
                // TODO
            }
        }
    }
}