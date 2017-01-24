using System;
using System.Collections.Generic;
using DxCore;
using DxCore.Core.Components.Basic;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using MapEditorLibrary.Controls;
using MapEditorLibrary.Core.Messaging;
using MapEditorLibrary.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NLog;
using WallNetCore.Cache.Advanced;
using WallNetCore.Validate;

namespace MapEditorLibrary.Core.Components
{
    public class MapCreatorComponent : DrawableComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ILoadingCache<Tile, Texture2D> tileTextureCache_ =
            CacheBuilder<Tile, Texture2D>.NewBuilder()
                .Build(tile => DxGame.Instance.Content.Load<Texture2D>(tile.Asset));

        private TilePosition? CurrentTilePosition
        {
            get
            {
                DxVector2 mousePosition = Mouse.GetState().Position;
                CameraService cameraService = DxGame.Instance.Service<CameraService>();
                DxVector2 worldSpacePosition = cameraService.UiOffsetToWorldCoordinates(mousePosition);

                TilePosition tilePosition;
                if(MapGrid.PositionForPoint(worldSpacePosition, out tilePosition))
                {
                    return tilePosition;
                }
                return null;
            }
        }

        private MapDescriptor.MapDescriptorBuilder MapBuilder { get; }

        private SingleElementLocalLoadingCache<MapDescriptor> MapDescriptorCache { get; }

        private MapGridComponent MapGrid { get; }

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

                Texture2D tileTexture = tileTextureCache_.Get(tile);

                spriteBatch.Draw(tileTexture, space, Color.White);
            }
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

        private void HandleAddTileToMapRequest(AddTileToMapRequest request)
        {
            TilePosition? tilePosition = CurrentTilePosition;
            if(!tilePosition.HasValue)
            {
                return;
            }
            RootUiService rootUi;
            if(!DxGame.Instance.ServiceProvider.TryGet(out rootUi))
            {
                Logger.Debug("Unable to find {0}", typeof(RootUiService));
                return;
            }
            Tile currentTile = rootUi.SelectedTile;
            if(ReferenceEquals(currentTile, null))
            {
                Logger.Debug("Ignoring {0} for {1}, no selected tile found", typeof(AddTileToMapRequest), tilePosition);
                return;
            }

            TileModel currentTileModel = rootUi.SelectedTileModel;
            tileTextureCache_.Put(currentTile, currentTileModel.Texture);
            MapBuilder.WithTile(tilePosition.Value, currentTile);
            MapDescriptorCache.Invalidate();
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

        private void HandleResetMapRequest(ResetMapRequest request)
        {
            MapBuilder.WithoutTiles();
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
    }
}