using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.DataStructures;
using DXGame.Core.Primitives;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /**

        <summary>
            Keeps track of FPS via calls to Draw (so it's totally accurate wooo)
        </summary>
    */
    [Serializable]
    [DataContract]
    public class FpsTracker : DrawableComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);
        private readonly LinkedList<TimeSpan> frameTimes_ = new LinkedList<TimeSpan>();
        private readonly Stopwatch stopWatch_;
        private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);
        public int FramesPerSecond { get; private set; }
        public override bool ShouldSerialize => false;

        public FpsTracker()
        {
            stopWatch_ = Stopwatch.StartNew();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var timeStamp = gameTime.TotalGameTime;
            UpdateFrameCount();

            if ((timeStamp - ONE_SECOND) > lastUpdated_)
            {
                // TODO: Have a widget draw this instead of logging it
                lastUpdated_ = timeStamp;
            }
        }

        private void UpdateFrameCount()
        {
            TimeSpan currentTime = stopWatch_.Elapsed;
            TimeSpan minimumCutOff = currentTime - ONE_SECOND;
            frameTimes_.AddLast(currentTime);
            ++FramesPerSecond;
            while (true)
            {
                var earliestTime = frameTimes_.First.Value;
                if (earliestTime < minimumCutOff)
                {
                    --FramesPerSecond;
                    frameTimes_.RemoveFirst();
                }
                else
                {
                    break;
                }
            }
        }
    }
}