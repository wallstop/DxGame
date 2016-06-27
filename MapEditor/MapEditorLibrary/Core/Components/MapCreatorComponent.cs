using System;
using System.Collections.Generic;
using DxCore;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Cache.Advanced;
using DxCore.Core.Utils.Validate;
using MapEditorLibrary.Controls;
using MapEditorLibrary.Core.Messaging;
using MapEditorLibrary.Core.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;

namespace MapEditorLibrary.Core.Components
{
    public class MapCreatorComponent : DrawableComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ICache<Tile, Texture2D> tileTextureCache_ =
            new LocalCache<Tile, Texture2D>(CacheBuilder<Tile, Texture2D>.NewBuilder());

        private MapDescriptor.MapDescriptorBuilder MapBuilder { get; }

        private MapGridComponent MapGrid { get; }

        private SingleElementLocalLoadingCache<MapDescriptor> MapDescriptorCache { get; }

        public MapCreatorComponent(MapGridComponent mapGrid)
        {
            Validate.Hard.IsNotNullOrDefault(mapGrid);
            MapGrid = mapGrid;
            MapBuilder = new MapDescriptor.MapDescriptorBuilder();
            MapBuilder.WithWidth(mapGrid.MapWidth);
            MapBuilder.WithHeight(mapGrid.MapHeight);
            MapBuilder.WithTileWidth(mapGrid.TileUnitWidth);
            MapBuilder.WithTileHeight(mapGrid.TileUnitHeight);

            MapDescriptorCache = new SingleElementLocalLoadingCache<MapDescriptor>(CacheBuilder<FastCacheKey, MapDescriptor>.NewBuilder(),() => MapBuilder.Build());
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<AddTileToMapRequest>(HandleAddTileToMapRequest);
            base.OnAttach();
        }

        private void HandleAddTileToMapRequest(AddTileToMapRequest request)
        {
            DxVector2 mousePosition = Mouse.GetState().Position;
            DxVector2 offset = DxGame.Instance.Model<CameraModel>().OffsetFromOrigin;
            mousePosition += offset;

            TilePosition tilePosition = MapGrid.PositionForPoint(mousePosition);
            Tile currentTile = DxGame.Instance.Model<RootUiModel>().SelectedTile;
            if(ReferenceEquals(currentTile, null))
            {
                Logger.Debug("Ignoring {0} for {1}, no selected tile found", typeof(AddTileToMapRequest), tilePosition);
                return;
            }

            TileModel currentTileModel = DxGame.Instance.Model<RootUiModel>().SelectedTileModel;
            tileTextureCache_.Put(currentTile, currentTileModel.Texture);
            MapBuilder.WithTile(tilePosition, currentTile);
            MapDescriptorCache.Invalidate();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            MapDescriptor mapDescriptor = MapDescriptorCache.Get();
            int tileWidth = MapGrid.TileWidth;
            int tileHeight = MapGrid.TileHeight;

            foreach(KeyValuePair<TilePosition, Tile> tilePositionAndTile in mapDescriptor.Tiles)
            {
                TilePosition tilePosition = tilePositionAndTile.Key;
                Tile tile = tilePositionAndTile.Value;
                DxRectangle space = new DxRectangle(tilePosition.X * tileWidth, tilePosition.Y * tileHeight, tileWidth,
                    tileHeight);

                Texture2D tileTexture = tileTextureCache_.Get(tile,
                    () => { throw new InvalidOperationException($"{tile} should have had a texture"); });

                spriteBatch.Draw(tileTexture, space, Color.White);
            }
        }
    }
}
