using System;
using System.Collections.Generic;
using DxCore;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
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
            MapBuilder.WithMapLayout(mapGrid.MapLayout);

            MapDescriptorCache =
                new SingleElementLocalLoadingCache<MapDescriptor>(
                    CacheBuilder<FastCacheKey, MapDescriptor>.NewBuilder(), () => MapBuilder.Build());
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<AddTileToMapRequest>(HandleAddTileToMapRequest);
            RegisterMessageHandler<RemoveTileFromMapRequest>(HandleRemoveTileFromMapRequest);
            RegisterMessageHandler<SaveMapRequest>(HandleSaveMapRequest);
            RegisterMessageHandler<LoadMapRequest>(HandleLoadMapRequest);
            RegisterMessageHandler<MapLayoutChanged>(HandleMapLayoutChanged);
            RegisterMessageHandler<ResetMapRequest>(HandleResetMapRequest);
            base.OnAttach();
        }

        private void HandleResetMapRequest(ResetMapRequest request)
        {
            MapBuilder.WithoutTiles();
            MapDescriptorCache.Invalidate();
        }

        private void HandleMapLayoutChanged(MapLayoutChanged mapLayoutChanged)
        {
            MapLayout newMapLayout = mapLayoutChanged.NewLayout;
            /* Cull tiles */
            foreach(TilePosition tilePosition in MapBuilder.Tiles.Keys)
            {
                if(tilePosition.X >= newMapLayout.Width)
                {
                    MapBuilder.WithoutTile(tilePosition);
                }
                else if(tilePosition.Y >= newMapLayout.Height)
                {
                    MapBuilder.WithoutTile(tilePosition);
                }
            }
            MapBuilder.WithMapLayout(newMapLayout);
            MapDescriptorCache.Invalidate();
        }

        private void HandleSaveMapRequest(SaveMapRequest saveMapRequest)
        {
            try
            {
                MapDescriptor mapDescriptor = MapDescriptorCache.Get();
                mapDescriptor.Save(saveMapRequest.FilePath);
            }
            catch(Exception e)
            {
                string errorMessage = $"Failed to save map to {saveMapRequest.FilePath}";
                Logger.Error(e, errorMessage);
                new ErrorNotification(errorMessage).Emit();
            }
        }

        private void HandleLoadMapRequest(LoadMapRequest loadMapRequest)
        {
            MapDescriptor loadedMap;
            try
            {
                loadedMap = MapDescriptor.StaticLoad(loadMapRequest.FilePath);
            }
            catch(Exception e)
            {
                string errorMessage = $"Failed to load map from {loadMapRequest.FilePath}";
                Logger.Error(e, errorMessage);
                new ErrorNotification(errorMessage).Emit();
                return;
            }
            MapDescriptorCache.Invalidate();
            MapBuilder.WithMapDescriptor(loadedMap);
            new MapChangedNotification(MapDescriptorCache.Get()).Emit();
        }

        private void HandleRemoveTileFromMapRequest(RemoveTileFromMapRequest request)
        {
            TilePosition? tilePosition = CurrentTilePosition;
            if(!tilePosition.HasValue)
            {
                return;
            }
            MapBuilder.WithoutTile(tilePosition.Value);
            MapDescriptorCache.Invalidate();
        }

        private void HandleAddTileToMapRequest(AddTileToMapRequest request)
        {
            TilePosition? tilePosition = CurrentTilePosition;
            if(!tilePosition.HasValue)
            {
                return;
            }
            Tile currentTile = DxGame.Instance.Model<RootUiModel>().SelectedTile;
            if(ReferenceEquals(currentTile, null))
            {
                Logger.Debug("Ignoring {0} for {1}, no selected tile found", typeof(AddTileToMapRequest), tilePosition);
                return;
            }

            TileModel currentTileModel = DxGame.Instance.Model<RootUiModel>().SelectedTileModel;
            tileTextureCache_.Put(currentTile, currentTileModel.Texture);
            MapBuilder.WithTile(tilePosition.Value, currentTile);
            MapDescriptorCache.Invalidate();
        }

        private TilePosition? CurrentTilePosition
        {
            get
            {
                DxVector2 mousePosition = Mouse.GetState().Position;
                CameraModel cameraModel = DxGame.Instance.Model<CameraModel>();
                DxVector2 worldSpacePosition = cameraModel.Invert(mousePosition);

                TilePosition tilePosition;
                if(MapGrid.PositionForPoint(worldSpacePosition, out tilePosition))
                {
                    return tilePosition;
                }
                return null;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            MapDescriptor mapDescriptor = MapDescriptorCache.Get();
            int tileWidth = MapGrid.MapLayout.TileWidth;
            int tileHeight = MapGrid.MapLayout.TileHeight;

            foreach(KeyValuePair<TilePosition, Tile> tilePositionAndTile in mapDescriptor.Tiles)
            {
                TilePosition tilePosition = tilePositionAndTile.Key;
                Tile tile = tilePositionAndTile.Value;
                DxRectangle space = new DxRectangle(tilePosition.X * tileWidth, tilePosition.Y * tileHeight, tileWidth,
                    tileHeight);

                Texture2D tileTexture = tileTextureCache_.Get(tile,
                    () => DxGame.Instance.Content.Load<Texture2D>(tile.Asset));

                spriteBatch.Draw(tileTexture, space, Color.White);
            }
        }
    }
}
