﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public DeveloperModel(DxGame game) 
            : base(game)
        {
            DrawPriority = DrawPriority.HUD_LAYER;
            var fpsTracker = new FpsWidget(DxGame);
            components_.Add(fpsTracker);
            devSwitch_ = new DeveloperSwitch(DxGame);
            components_.Add(devSwitch_);
            var mapTreeWidget = new CollisionTreeWidget<MapCollidableComponent>(game, () => DxGame.Model<MapModel>().Map.Collidables);
            components_.Add(mapTreeWidget);
            var boundingBoxWidget = new BoundingBoxWidget(game);
            components_.Add(boundingBoxWidget);
            var teamCounterWidget = new TeamCounterWidget(game);
            components_.Add(teamCounterWidget);
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
