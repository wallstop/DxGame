using System.Collections.Generic;
using System.IO;
using System.Linq;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Map;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using DXGame.Core.Messaging;

namespace DXGame.Core.Models
{
    /* TODO: Rename to be LevelModel? */
    public class MapModel : Model
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly string MAP_PATH = "Content/Map/";
        private readonly List<Map.Map> maps_ = new List<Map.Map>();
        public DxRectangle RandomSpawnLocation => Map.RandomSpawnLocation;
        public DxVector2 PlayerSpawn => Map.PlayerSpawn;
        public DxRectangle MapBounds => Map.MapDescriptor.Size * Map.MapDescriptor.Scale;
        public Map.Map Map { get; private set; }
        public override bool ShouldSerialize => false;

        private int currentMapIndex_ = 1;

        public MapModel()
        {
            DrawPriority = DrawPriority.MAP;
            MessageHandler.RegisterMessageHandler<MapRotationRequest>(HandleMapRotationRequest);
            MessageHandler.RegisterMessageHandler<MapRotationNotification>(HandleMapFinishedLoading);
        }

        public void HandleMapRotationRequest(MapRotationRequest mapRotationRequest)
        {
            if(ReferenceEquals(mapRotationRequest, null))
            {
                LOG.Info($"Received null {typeof(MapRotationRequest)}, not rotating map.");
                return;
            }

            currentMapIndex_ = currentMapIndex_.WrappedAdd(1, maps_.Count);
            Map = maps_[currentMapIndex_];
            MapRotationNotification mapRotationNotification = new MapRotationNotification();
            DxGame.Instance.BroadcastMessage<MapRotationNotification>(mapRotationNotification);
        }

        public void HandleMapFinishedLoading(MapRotationNotification mapRotationNotification)
        {
            /* TODO: Change to be... spawner based or some shit */

            /* Try and put it on a surface */
            List<MapCollidableComponent> mapTiles = Map.Collidables.Elements.ToList();
            DxVector2 mapTransitionLocation;
            do
            {
                MapCollidableComponent mapTile = ThreadLocalRandom.Current.FromCollection(mapTiles);
                float x = ThreadLocalRandom.Current.NextFloat(mapTile.Spatial.Space.X,
                    mapTile.Spatial.Space.X + mapTile.Spatial.Space.Width);
                mapTransitionLocation = new DxVector2(x, mapTile.Spatial.Space.Y - 7.5f);
            } while(
                Map.Collidables.InRange(new DxRectangle(mapTransitionLocation.X, mapTransitionLocation.Y, 5, 5)).Any());

            PositionalComponent transitionSpatial =
                PositionalComponent.Builder().WithPosition(mapTransitionLocation).Build();
            MapTransition mapTransition = new MapTransition(transitionSpatial);
            GameObject mapTransitionObject =
                GameObject.Builder().WithComponents(transitionSpatial, mapTransition).Build();
            DxGame.Instance.AddAndInitializeGameObject(mapTransitionObject);
        }

        public override void LoadContent()
        {
            maps_.Clear();
            var maps =
                Directory.EnumerateFiles(MAP_PATH)
                    .Where(
                        path =>
                            System.IO.Path.HasExtension(path) &&
                            (System.IO.Path.GetExtension(path)?.Equals(MapDescriptor.MapExtension) ?? false))
                    .Select(MapDescriptor.StaticLoad)
                    .Select(mapDescriptor => new Map.Map(mapDescriptor));
            maps_.AddRange(maps);
            Validate.IsNotEmpty(maps_, $"Failed to find maps! Check {MAP_PATH} for valid descriptors");
            base.LoadContent();
        }

        public override void Initialize()
        {
            foreach(var map in maps_)
            {
                map.LoadContent();
                map.Initialize();
            }
            base.Initialize();
            MapRotationRequest mapRotationRequest = new MapRotationRequest();
            DxGame.Instance.BroadcastMessage<MapRotationRequest>(mapRotationRequest);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Map.Draw(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
        }
    }
}