using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.Level;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using DXGame.TowerGame.Enemies;

namespace DXGame.TowerGame.Level
{
    /**
        <summary>
            Super-simple level "strategy".
            Walks through the Map directory, pulls all maps, adds random small and large box spawners, level end spawners.
        </summary>
    */
    [DataContract]
    [Serializable]
    public class SimpleRotatingLevelProgression : Component, ILevelProgressionStrategy
    {
        private static readonly string MAP_PATH = "Content/Map/";
        private readonly List<Map> maps_ = new List<Map>();

        private int currentMapIndex_ = 0;

        private Map CurrentMap => maps_[currentMapIndex_];

        public Core.Level.Level InitialLevel { get; private set; }

        public Core.Level.Level DetermineNextLevel(Core.Level.Level currentLevel)
        {
            /* 
                We don't care what the last level was, we just want to rotate maps. 
                If we truly cared, we could do some validation that they provided level was the last one we generated, but we don't! 
                #yolo 
            */
            return GenerateLevel();
        }

        private Core.Level.Level GenerateLevel()
        {
            ++currentMapIndex_;
            if(currentMapIndex_ == maps_.Count)
            {
                return GenerateLastLevel();
            }

            Spawner smallBoxSpawner = SpawnerFactory.SimpleSmallBoxSpawner();
            Spawner largeBoxSpawner = SpawnerFactory.SimpleLargeBoxSpawner();
            Spawner levelEndSpawner = GenerateMapTransitionSpawner();

            Core.Level.Level generatedLevel =
                Core.Level.Level.Builder()
                    .WithMap(CurrentMap)
                    .WithSpawners(smallBoxSpawner, largeBoxSpawner, levelEndSpawner)
                    .Build();

            return generatedLevel;
        }

        private Core.Level.Level GenerateLastLevel()
        {
            // TODO generate an "LOL YOU WON SILLY GOOSE" level / text with floating & stupid text that you can't exit
            throw new NotImplementedException();
        }

        private Spawner GenerateMapTransitionSpawner()
        {
            Spawner.SpawnerBuilder levelEndSpawner = Spawner.Builder();
            /* Try and put it on a surface */
            Map map = CurrentMap;
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
            levelEndSpawner.WithSpawnArea(new DxRectangle(mapTransitionLocation.X, mapTransitionLocation.Y, 1, 1));

            PositionalComponent transitionSpatial =
                PositionalComponent.Builder().WithPosition(mapTransitionLocation).Build();
            MapTransition mapTransition = new MapTransition(transitionSpatial);
            GameObject mapTransitionObject =
                GameObject.Builder().WithComponents(transitionSpatial, mapTransition).Build();

            LevelEndTrigger levelEndTrigger = new LevelEndTrigger(mapTransitionObject);
            SpawnTrigger spawnTrigger = levelEndTrigger.CheckForLevelEndRequest;

            levelEndSpawner.WithSpawnTrigger(spawnTrigger);
            return levelEndSpawner.Build();
        }

        public override void LoadContent()
        {
            maps_.Clear();
            var maps =
                Directory.EnumerateFiles(MAP_PATH)
                    .Where(
                        path =>
                            Path.HasExtension(path) &&
                            (Path.GetExtension(path)?.Equals(MapDescriptor.MapExtension) ?? false))
                    .Select(MapDescriptor.StaticLoad)
                    .Select(mapDescriptor => new Map(mapDescriptor));
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
            InitialLevel = GenerateLevel();
            base.Initialize();
        }
    }

    /**
        <summary>
            Simple data-wrapper class for constructed MapTransitionObjects. 
            We wrap them in a class so they can be serialized to the network (lambdas cannot serialize well if they're closures)
        </summary>
    */

    [DataContract]
    [Serializable]
    internal class LevelEndTrigger
    {
        private readonly GameObject mapTransitionObject_;
        private bool alreadyTriggered_;

        public LevelEndTrigger(GameObject mapTransitionObject)
        {
            Validate.IsNotNullOrDefault(mapTransitionObject,
                StringUtils.GetFormattedNullOrDefaultMessage(this, "MapTransitionObject"));
            mapTransitionObject_ = mapTransitionObject;
        }

        public Tuple<bool, GameObject> CheckForLevelEndRequest(DxGameTime gameTime)
        {
            if(alreadyTriggered_)
            {
                return Tuple.Create<bool, GameObject>(false, null);
            }
            EventModel eventModel = DxGame.Instance.Model<EventModel>();
            if(ReferenceEquals(eventModel, null))
            {
                return Tuple.Create<bool, GameObject>(false, null);
            }
            EventRequest levelEndRequestRequest = EventRequest.Builder().WithType<LevelEndRequest>().Build();
            List<Event> levelEndEvents = eventModel.EventsFor(levelEndRequestRequest, gameTime);
            List<LevelEndRequest> levelEndRequests =
                levelEndEvents.Select(endEvent => endEvent.Message as LevelEndRequest).ToList();
            if(levelEndRequests.Any())
            {
                alreadyTriggered_ = true;
                return Tuple.Create(true, mapTransitionObject_);
            }
            return Tuple.Create<bool, GameObject>(false, null);
        }
    }
}