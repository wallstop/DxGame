using System;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public class FpsTracker : DrawableComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);
        private readonly SortedList<TimeSpan> frameTimes_ = new SortedList<TimeSpan>();
        private readonly Stopwatch stopWatch_;
        private TimeSpan lastUpdated_ = TimeSpan.FromSeconds(0);
        public int FramesPerSecond => frameTimes_.Count;
        public override bool ShouldSerialize => false;

        public FpsTracker(DxGame game) : base(game)
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
            frameTimes_.RemoveBelow(minimumCutOff);
            frameTimes_.Add(currentTime);
        }
    }
}