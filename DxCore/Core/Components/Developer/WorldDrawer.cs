using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Developer.Farseer;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    public class WorldDrawer : DrawableComponent
    {
        private DebugViewXNA DebugView { get; set; }

        public WorldDrawer()
        {
            DrawPriority = DrawPriority.UserPrimitives;
        }

        public override void LoadContent()
        {
            WorldModel worldModel = DxGame.Instance.Model<WorldModel>();
            DebugView = new DebugViewXNA(worldModel.World)
            {
                DefaultShapeColor = Color.Blue,
                StaticShapeColor = Color.Blue
            };
            DebugView.AppendFlags(DebugViewFlags.AABB);
            DebugView.AppendFlags(DebugViewFlags.PerformanceGraph);
            DebugView.AppendFlags(DebugViewFlags.Controllers);
            DebugView.AppendFlags(DebugViewFlags.ContactPoints);
            DebugView.AppendFlags(DebugViewFlags.DebugPanel);
            DebugView.AppendFlags(DebugViewFlags.PolygonPoints);
            DebugView.AppendFlags(DebugViewFlags.Shape);
            DebugView.LoadContent(DxGame.Instance.GraphicsDevice, DxGame.Instance.Content);
            base.LoadContent();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            DeveloperModel devModel = DxGame.Instance.Model<DeveloperModel>();
            if(devModel?.DeveloperMode == DeveloperMode.NotSoOn)
            {
                return;
            }
            DxRectangle screen = DxGame.Instance.ScreenRegion;
            CameraModel cameraModel = DxGame.Instance.Model<CameraModel>();

            // TODO: Fix
            Matrix transform = Matrix.CreateOrthographicOffCenter(-screen.X * WorldModel.DxToFarseerScale,
                WorldModel.DxToFarseerScale * (1280 - screen.X), WorldModel.DxToFarseerScale * (720- screen.Y),
                -screen.Y * WorldModel.DxToFarseerScale, 0, 1f);
            DebugView.RenderDebugData(ref transform);
        }
    }
}
