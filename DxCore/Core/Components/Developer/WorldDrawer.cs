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
        private static readonly DebugViewFlags DebugFlags = DebugViewFlags.AABB | DebugViewFlags.PerformanceGraph |
                                                            DebugViewFlags.Controllers | DebugViewFlags.ContactPoints |
                                                            DebugViewFlags.DebugPanel | DebugViewFlags.PolygonPoints |
                                                            DebugViewFlags.Shape;

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
            DebugView.LoadContent(DxGame.Instance.GraphicsDevice, DxGame.Instance.Content);
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            DeveloperModel devModel = DxGame.Instance.Model<DeveloperModel>();
            if(devModel?.DeveloperMode != DeveloperMode.FullOn)
            {
                DebugView.RemoveFlags(DebugFlags);
            }
            else
            {
                DebugView.AppendFlags(DebugFlags);
            }
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
                WorldModel.DxToFarseerScale * (1280 - screen.X), WorldModel.DxToFarseerScale * (720 - screen.Y),
                -screen.Y * WorldModel.DxToFarseerScale, 0, 1f);
            DebugView.RenderDebugData(ref transform);
        }
    }
}
