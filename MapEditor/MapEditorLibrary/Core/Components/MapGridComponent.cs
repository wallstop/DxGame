﻿using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;
using MapEditorLibrary.Core.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditorLibrary.Core.Components
{
    public class MapGridComponent : DrawableComponent
    {
        public MapLayout MapLayout { get; private set; }

        public Color Color { get; set; } = Color.White;

        public DxRectangle Bounds => MapLayout.Bounds;

        public MapGridComponent(int tileWidth, int tileHeight, int mapWidth, int mapHeight)
        {
            MapLayout =
                MapLayout.Builder()
                    .WithWidth(mapWidth)
                    .WithHeight(mapHeight)
                    .WithTileWidth(tileWidth)
                    .WithTileHeight(tileHeight)
                    .Build();

            new UpdateCameraBounds(Bounds).Emit();
        }

        public bool PositionForPoint(DxVector2 point, out TilePosition tilePosition)
        {
            if(!Validate.Check.IsTrue(Bounds.Contains(point)))
            {
                tilePosition = default(TilePosition);
                return false;
            }

            int x = (int) Math.Round(point.X) / MapLayout.TileWidth;
            int y = (int) Math.Round(point.Y) / MapLayout.TileHeight;

            tilePosition = new TilePosition(x, y);
            return true;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<MapLayoutChanged>(HandleMapLayoutChanged);
            base.OnAttach();
        }

        private void HandleMapLayoutChanged(MapLayoutChanged mapLayoutChanged)
        {
            MapLayout = mapLayoutChanged.NewLayout;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            for(int i = 0; i <= MapLayout.Width; ++i)
            {
                spriteBatch.DrawLine(new DxVector2(MapLayout.TileWidth * i, 0),
                    new DxVector2(MapLayout.TileWidth * i, MapLayout.Height * MapLayout.TileHeight), Color, 1, 0.9f);
            }
            for(int j = 0; j <= MapLayout.Height; ++j)
            {
                spriteBatch.DrawLine(new DxVector2(0, j * MapLayout.TileHeight),
                    new DxVector2(MapLayout.Width * MapLayout.TileWidth, j * MapLayout.TileHeight), Color, 1, 0.9f);
            }
        }
    }
}
