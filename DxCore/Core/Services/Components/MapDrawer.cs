﻿using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Services.Components
{
    public sealed class MapDrawer : DrawableComponent
    {
        private Func<Level.Level> LevelProducer { get; }

        public MapDrawer(Func<Level.Level> levelProducer)
        {
            DrawPriority = DrawPriority.Map;
            Validate.Hard.IsNotNull(levelProducer);
            LevelProducer = levelProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            LevelProducer().Map.Draw(spriteBatch, gameTime);
        }

        public override void Initialize()
        {
            LevelProducer().Create();
        }
    }
}