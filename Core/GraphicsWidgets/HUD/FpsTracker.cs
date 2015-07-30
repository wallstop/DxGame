using System;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public class FpsTracker : DrawableComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (FpsTracker));
        private readonly SortedList<TimeSpan> frameTimes_ = new SortedList<TimeSpan>();
        private readonly TimeSpan oneSecond_ = TimeSpan.FromSeconds(1);
        private readonly Stopwatch stopWatch_;
        public TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);
        public int FramesPerSecond => frameTimes_.Count;

        public FpsTracker(DxGame game) : base(game)
        {
            stopWatch_ = Stopwatch.StartNew();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var timeStamp = gameTime.TotalGameTime;
            UpdateFrameCount();

            if ((timeStamp - oneSecond_) > lastUpdated_)
            {
                // TODO: Have a widget draw this instead of logging it
                lastUpdated_ = timeStamp;
                LOG.Info($"FPS: {FramesPerSecond}");
            }
        }

        private void UpdateFrameCount()
        {
            TimeSpan currentTime = stopWatch_.Elapsed;
            TimeSpan minimumCutOff = currentTime - oneSecond_;
            frameTimes_.RemoveBelow(minimumCutOff);
            frameTimes_.Add(currentTime);
        }
    }
}