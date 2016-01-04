using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Level;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.TowerGame.Level
{
    [DataContract]
    [Serializable]
    public class SimpleRotatingLevelProgression : Component, ILevelProgressionStrategy
    {
        private static readonly string MAP_PATH = "Content/Map/";

        private int currentMapIndex_ = 0;
        private readonly List<Core.Map.Map> maps_ = new List<Core.Map.Map>();

        public Core.Level.Level DetermineNextLevel(Core.Level.Level currentLevel)
        {
            throw new NotImplementedException();
        }

        private Core.Level.Level GenerateLevel()
        {
            ++currentMapIndex_;
            if(currentMapIndex_ == maps_.Count)
            {
                return GenerateLastLevel();
            }

            Spawner smallBoxSpawner = Enemies.SpawnerFactory.SimpleSmallBoxSpawner();
            Spawner largeBoxSpawner = Enemies.SpawnerFactory.SimpleLargeBoxSpawner();
            // TODO: Spawn trigger & spawn area for MapTransition
            Spawner levelEndSpawner = Spawner.Builder().WithSpawnArea()

            // TODO
            throw new NotImplementedException();
        }

        private Core.Level.Level GenerateLastLevel()
        {
            // TODO
            throw new NotImplementedException();
        }

        private GameObject GenerateMapTransitionObject()
        {
            /* Try and put it on a surface */
            Map map = maps_[currentMapIndex_];
            List<MapCollidableComponent> mapTiles = map.Collidables.Elements.ToList();
            DxVector2 mapTransitionLocation;
            do
            {
                MapCollidableComponent mapTile = ThreadLocalRandom.Current.FromCollection(mapTiles);
                float x = ThreadLocalRandom.Current.NextFloat(mapTile.Spatial.Space.X,
                    mapTile.Spatial.Space.X + mapTile.Spatial.Space.Width);
                mapTransitionLocation = new DxVector2(x, mapTile.Spatial.Space.Y - 7.5f);
            } while(
                map.Collidables.InRange(new DxRectangle(mapTransitionLocation.X, mapTransitionLocation.Y, 5, 5)).Any());

            PositionalComponent transitionSpatial =
                PositionalComponent.Builder().WithPosition(mapTransitionLocation).Build();
            MapTransition mapTransition = new MapTransition(transitionSpatial);
            GameObject mapTransitionObject =
                GameObject.Builder().WithComponents(transitionSpatial, mapTransition).Build();
            return mapTransitionObject;
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
                    .Select(mapDescriptor => new Core.Map.Map(mapDescriptor));
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
        }
    }
}
