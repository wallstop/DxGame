using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Developer;
using DXGame.Core.Components.Utils;
using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Models
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
            DrawPriority = DrawPriority.HUD_LAYER;
            var fpsTracker = new FpsWidget();
            components_.Add(fpsTracker);
            devSwitch_ = new DeveloperSwitch();
            components_.Add(devSwitch_);
            var mapTreeWidget = new CollisionTreeWidget<MapCollidableComponent>(() => DxGame.Instance.Model<MapModel>().Map.Collidables);
            components_.Add(mapTreeWidget);
            var boundingBoxWidget = new BoundingBoxWidget();
            components_.Add(boundingBoxWidget);
            var teamCounterWidget = new TeamCounterWidget();
            components_.Add(teamCounterWidget);
            var timePerFrameBackground = new TimePerFrameGraphBackground();
            components_.Add(timePerFrameBackground);
            var timePerFrameGraph = new TimePerFrameGraph();
            DxGame.Instance.AddAndInitializeComponents(timePerFrameGraph);
        }

        public override bool ShouldSerialize => false;

        public override void Initialize()
        {
            foreach (var component in from object element in components_ select element as Component)
            {
                component?.Initialize();
            }
            base.Initialize();
        }

        public override void LoadContent()
        {
            foreach (var component in from object element in components_ select element as Component)
            {
                component?.LoadContent();
            }
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            foreach (var updateable in components_.Processables)
            {
                updateable.Process(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if (devSwitch_.DeveloperMode != DeveloperMode.NotSoOn)
            {
                foreach (var drawable in components_.Drawables)
                {
                    drawable.Draw(spriteBatch, gameTime);
                }
            }
            base.Draw(spriteBatch, gameTime);
        }
    }
}
