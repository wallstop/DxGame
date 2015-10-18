using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.DataStructures;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
{
    [Serializable]
    [DataContract]
    public class TimePerFrameGraphBackground : DrawableComponent
    {
        public TimePerFrameGraphBackground(DxGame game) 
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var screenRegion = DxGame.ScreenRegion;
            // TODO: Fix whatever weird math is being done with the screen region to make drawing things "sane"
            var drawLocation = new Vector2(Math.Abs(screenRegion.X) + (screenRegion.Width / 2.0f),
                Math.Abs(screenRegion.Y));
            var drawSize = new Vector2(TimePerFrameGraph.Size.X, TimePerFrameGraph.Size.Y);
            var blackTexture = TextureFactory.TextureForColor(Color.Black);
            const float transparencyWeight = 0.8f;
            var transparency = ColorFactory.Transparency(transparencyWeight);
            /* Draw a neato transparent box behind the text to make the text "pop" */
            spriteBatch.Draw(blackTexture, color: transparency,
                destinationRectangle:
                    new Rectangle((int)drawLocation.X, (int)drawLocation.Y, (int)drawSize.X, (int)drawSize.Y));
        }
    }

    [Serializable]
    [DataContract]
    public class TimePerFrameGraph : DrawableComponent
    {
        private static readonly TimeSpan STRETCH_TO_KEEP = TimeSpan.FromSeconds(2);
        internal static readonly Point Size = new Point(200, 50);

        [DataMember]
        private SortedList<DxGameTime> GameTimes { get; }

        [DataMember]
        private Graph Graph { get; set; }

        public TimePerFrameGraph(DxGame game) 
            : base(game)
        {
            GameTimes = new SortedList<DxGameTime>();
            Graph = new Graph(DxGame.GraphicsDevice, Size)
            {
                MaxValue = (float)DxGame.TargetElapsedTime.TotalMilliseconds,
                Type = Graph.GraphType.Line
            };
            DrawPriority = DrawPriority.USER_PRIMITIVES;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var developerModel = DxGame.Model<DeveloperModel>();
            if (developerModel.DeveloperMode == DeveloperMode.FullOn)
            {
                GameTimes.Add(gameTime);
                DxGameTime removalGameTime = new DxGameTime(gameTime.TotalGameTime - STRETCH_TO_KEEP,
                    gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
                GameTimes.RemoveBelow(removalGameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var developerModel = DxGame.Model<DeveloperModel>();
            if (developerModel.DeveloperMode == DeveloperMode.FullOn)
            {
                var screenRegion = DxGame.ScreenRegion;
                // TODO: Fix whatever weird math is being done with the screen region to make drawing things "sane"
                var drawLocation = new Vector2((screenRegion.Width / 2.0f), 0);
                drawLocation.Y += Size.Y;
                Graph.Position = drawLocation;
                var points = GameTimes.Select(time =>

                {
                    var pointValue = (float) time.ElapsedGameTime.TotalMilliseconds;
                    var scale = Math.Min(1.0f, pointValue / DxGame.TargetElapsedTime.TotalMilliseconds);
                    var color = Color.White;
                    color.R = (byte)(color.R * scale);
                    color.B = (byte) 0;
                    color.G = (byte) (color.G * (1.0 - scale));
                    return Tuple.Create(pointValue, color);
                }
                
                ) .ToList();

                Graph.Draw(points);
            }
        }
    }
}
