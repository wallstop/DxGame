using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Developer.Farseer;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
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
            WorldService worldService = DxGame.Instance.Service<WorldService>();
            DebugView = new DebugViewXNA(worldService.World)
            {
                DefaultShapeColor = Color.Blue,
                StaticShapeColor = Color.Blue
            };
            DebugView.LoadContent(DxGame.Instance.GraphicsDevice, DxGame.Instance.Content);
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            DeveloperService devService = DxGame.Instance.Service<DeveloperService>();
            if(devService?.DeveloperMode != DeveloperMode.FullOn)
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
            DeveloperService devService = DxGame.Instance.Service<DeveloperService>();
            if(devService?.DeveloperMode == DeveloperMode.NotSoOn)
            {
                return;
            }
            DxRectangle screen = DxGame.Instance.ScreenRegion;
            CameraService cameraService = DxGame.Instance.Service<CameraService>();

            // TODO: Fix
            Matrix transform = Matrix.CreateOrthographicOffCenter(-screen.X * WorldService.DxToFarseerScale,
                WorldService.DxToFarseerScale * (1280 - screen.X), WorldService.DxToFarseerScale * (720 - screen.Y),
                -screen.Y * WorldService.DxToFarseerScale, 0, 1f);
            DebugView.RenderDebugData(ref transform);
        }
    }
}
