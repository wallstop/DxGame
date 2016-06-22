using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.GraphicsWidgets;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    [Serializable]
    [DataContract]
    public class TimePerFrameGraphBackground : DrawableComponent
    {
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var developerModel = DxGame.Instance.Model<DeveloperModel>();
            if(developerModel?.DeveloperMode == DeveloperMode.FullOn)
            {
                var screenRegion = DxGame.Instance.ScreenRegion;
                // TODO: Fix whatever weird math is being done with the screen region to make drawing things "sane"
                var drawLocation = new Vector2(Math.Abs(screenRegion.X) + screenRegion.Width / 2.0f,
                    Math.Abs(screenRegion.Y));
                var drawSize = new Vector2(TimePerFrameGraph.Size.X, TimePerFrameGraph.Size.Y);
                var blackTexture = TextureFactory.TextureForColor(Color.Black);
                const float transparencyWeight = 0.8f;
                var transparency = ColorFactory.Transparency(transparencyWeight);
                /* Draw a neato transparent box behind the text to make the text "pop" */
                spriteBatch.Draw(blackTexture, color: transparency,
                    destinationRectangle:
                        new Rectangle((int) drawLocation.X, (int) drawLocation.Y, (int) drawSize.X, (int) drawSize.Y));
            }
        }
    }

    [Serializable]
    [DataContract]
    public class TimePerFrameGraph : DrawableComponent
    {
        private static readonly TimeSpan STRETCH_TO_KEEP = TimeSpan.FromSeconds(2);
        internal static readonly Point Size = new Point(200, 50);

        [DataMember]
        private LinkedList<DxGameTime> GameTimes { get; }

        [DataMember]
        private Graph Graph { get; set; }

        public override bool ShouldSerialize => false;

        public TimePerFrameGraph()
        {
            GameTimes = new LinkedList<DxGameTime>();
            Graph = new Graph(DxGame.Instance.GraphicsDevice, Size)
            {
                MaxValue = (float) DxGame.Instance.TargetElapsedTime.TotalMilliseconds,
                Type = Graph.GraphType.Line
            };
            DrawPriority = DrawPriority.UserPrimitives;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var developerModel = DxGame.Instance.Model<DeveloperModel>();
            if(developerModel?.DeveloperMode == DeveloperMode.FullOn)
            {
                GameTimes.AddLast(gameTime);
                var removeBefore = gameTime.TotalGameTime - STRETCH_TO_KEEP;

                while(true)
                {
                    var firstGameTime = GameTimes.First.Value;
                    if(firstGameTime.TotalGameTime < removeBefore)
                    {
                        GameTimes.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var developerModel = DxGame.Instance.Model<DeveloperModel>();
            if(developerModel?.DeveloperMode == DeveloperMode.FullOn)
            {
                var screenRegion = DxGame.Instance.ScreenRegion;
                /* Draw direction to the screen buffer - there currently is no translation matrix for direct GPU primitives */
                var drawLocation = new Vector2(screenRegion.Width / 2.0f, 0);
                drawLocation.Y += Size.Y;
                Graph.Position = drawLocation;
                var points = GameTimes.Select(time =>

                {
                    var pointValue = (float) time.ElapsedGameTime.TotalMilliseconds;
                    var scale = Math.Min(1.0f, pointValue / DxGame.Instance.TargetElapsedTime.TotalMilliseconds);
                    var color = Color.White;
                    /* 
                        Scale our colors nicely towards "hotness". White starts as straight 255 values, so we can scale red up by 
                        the percent of a "60 fps frame" each frame is, and green up by the inverse of that, resulting in a decent gradiant 
                    */
                    color.R = (byte) (color.R * scale);
                    color.B = 0;
                    color.G = (byte) (color.G * (1.0 - scale));
                    return Tuple.Create(pointValue, color);
                }).ToList();

                Graph.Draw(points);
            }
        }
    }
}
