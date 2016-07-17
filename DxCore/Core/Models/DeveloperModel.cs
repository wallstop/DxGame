using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Developer;
using DxCore.Core.GraphicsWidgets.HUD;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Models
{
    /**
        For debug purposes only :^)

        <summary> Simple debug-type Model used to display useful information in-game. Should not be used in production. </summary>
    */

    public class DeveloperModel : Model
    {
        private readonly GameElementCollection components_ = new GameElementCollection();
        private readonly DeveloperSwitch devSwitch_;

        public DeveloperMode DeveloperMode => devSwitch_.DeveloperMode;

        public DeveloperModel()
        {
            DrawPriority = DrawPriority.HudLayer;
            var fpsTracker = new FpsWidget();
            components_.Add(fpsTracker);
            devSwitch_ = new DeveloperSwitch();
            WorldDrawer worldDrawer = new WorldDrawer();
            worldDrawer.Create();
            var teamCounterWidget = new TeamCounterWidget();
            components_.Add(teamCounterWidget);
            var timePerFrameBackground = new TimePerFrameGraphBackground();
            components_.Add(timePerFrameBackground);
            TimePerFrameGraph timePerFrameGraph = new TimePerFrameGraph();
            /* We need to let DxGame handle the TimePerFrameGraph - it draws at a different draw priority than we do */
            EntityCreatedMessage timePerFrameGraphCreated = new EntityCreatedMessage(timePerFrameGraph);
            timePerFrameGraphCreated.Emit();
            var keysPressed = new KeysPressedWidget();
            components_.Add(keysPressed);
            HealthAdjustor healthAdjustor = new HealthAdjustor();
            components_.Add(healthAdjustor);
        }

        public override void Initialize()
        {
            foreach(var component in from object element in components_ select element as Component)
            {
                component?.Initialize();
            }
            devSwitch_.Initialize();
            base.Initialize();
        }

        public override void LoadContent()
        {
            foreach(var component in from object element in components_ select element as Component)
            {
                component?.LoadContent();
            }
            devSwitch_.LoadContent();
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            devSwitch_.Process(gameTime);
            if(devSwitch_.DeveloperMode != DeveloperMode.NotSoOn)
            {
                foreach(var updateable in components_.Processables)
                {
                    updateable.Process(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(devSwitch_.DeveloperMode != DeveloperMode.NotSoOn)
            {
                foreach(var drawable in components_.Drawables)
                {
                    drawable.Draw(spriteBatch, gameTime);
                }
            }
            base.Draw(spriteBatch, gameTime);
        }
    }
}