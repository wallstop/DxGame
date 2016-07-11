using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Babel.Components.Waves;
using Babel.Enemies;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Basic;
using DxCore.Core.Events;
using DxCore.Core.Level;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace Babel.Level
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

        private int currentMapIndex_ = -1;

        private Map CurrentMap => maps_[currentMapIndex_];

        public DxCore.Core.Level.Level InitialLevel { get; private set; }

        public DxCore.Core.Level.Level DetermineNextLevel(DxCore.Core.Level.Level currentLevel)
        {
            /* 
                We don't care what the last level was, we just want to rotate maps. 
                If we truly cared, we could do some validation that they provided level was the last one we generated, but we don't! 
                #yolo 
            */
            return GenerateLevel();
        }

        public void Init()
        {
            LoadContent();
            Initialize();
        }

        private DxCore.Core.Level.Level GenerateLevel()
        {
            if(currentMapIndex_ == maps_.Count)
            {
                return GenerateLastLevel();
            }
            ++currentMapIndex_;

            Spawner levelEndSpawner = GenerateMapTransitionSpawner();
            Spawner waveBasedBoxSpawner = SpawnerFactory.WaveBasedSmallBoxSpawner();
            Spawner waveNotifierSpawner = GenerateWaveChangeNotifierSpawner();
            Spawner waveEmitterSpawner = GenerateWaveEmitterSpawner();

            DxCore.Core.Level.Level generatedLevel =
                DxCore.Core.Level.Level.Builder()
                    .WithMap(CurrentMap)
                    .WithSpawners(levelEndSpawner, waveNotifierSpawner, waveEmitterSpawner, waveBasedBoxSpawner)
                    .Build();

            return generatedLevel;
        }

        private DxCore.Core.Level.Level GenerateLastLevel()
        {
            GameObject bouncingStupidText = GameObject.Builder().Build();

            DxRectangle mapSize = new DxRectangle(0, 0, 1280, 720);

            int tileWidth = 50;
            int tileHeight = 50;
            int widthInTiles = 1280 / tileWidth + 1;
            int heightInTiles = 700 / tileHeight;

            MapDescriptor winningMapDescriptor =
                MapDescriptor.Builder()
                    .WithHeight(heightInTiles)
                    .WithWidth(widthInTiles)
                    .WithTileWidth(tileWidth)
                    .WithTileHeight(tileHeight)
                    .Build();

            Map winningMap = new Map(winningMapDescriptor);
            winningMap.LoadContent();
            winningMap.Initialize();

            Spawner textSpawner = new RandomSpawner(mapSize, Spawner.ImmediateSpawnTriggerFor(bouncingStupidText));

            DxCore.Core.Level.Level winningLevel =
                DxCore.Core.Level.Level.Builder().WithMap(winningMap).WithSpawner(textSpawner).Build();

            return winningLevel;
        }

        private Spawner GenerateWaveChangeNotifierSpawner()
        {
            Spawner.SpawnerBuilder waveNotifierSpawnerBuilder = Spawner.Builder();

            WaveChangeNotifier waveChangeNotifier = new WaveChangeNotifier();
            GameObject waveNotifierObject = GameObject.Builder().WithComponent(waveChangeNotifier).Build();
            WaveNotifierTrigger waveNotifierTrigger = new WaveNotifierTrigger(waveNotifierObject);
            SpawnTrigger spawnTrigger = waveNotifierTrigger.CheckForInitialSpawn;

            waveNotifierSpawnerBuilder.WithSpawnTrigger(spawnTrigger);
            return waveNotifierSpawnerBuilder.Build();
        }

        private Spawner GenerateWaveEmitterSpawner()
        {
            Spawner.SpawnerBuilder waveNotifierSpawnerBuilder = Spawner.Builder();

            GameObject waveNotifierObject = WaveEmitter.NewWaveEmitter();
            WaveNotifierTrigger waveNotifierTrigger = new WaveNotifierTrigger(waveNotifierObject);
            SpawnTrigger spawnTrigger = waveNotifierTrigger.CheckForInitialSpawn;

            waveNotifierSpawnerBuilder.WithSpawnTrigger(spawnTrigger);
            return waveNotifierSpawnerBuilder.Build();
        }

        private Spawner GenerateMapTransitionSpawner()
        {
            Spawner.SpawnerBuilder levelEndSpawner = Spawner.Builder();
            /* Try and put it on a surface */
            Map map = CurrentMap;
            List<MapTile> mapTiles = map.TileSpatialTree.Elements.ToList();
            DxVector2 mapTransitionLocation;
            do
            {
                MapTile mapTile = ThreadLocalRandom.Current.FromCollection(mapTiles);
                float x = ThreadLocalRandom.Current.NextFloat(mapTile.Space.X, mapTile.Space.X + mapTile.Space.Width);
                mapTransitionLocation = new DxVector2(x, mapTile.Space.Y - 7.5f);
            } while(
                map.TileSpatialTree.InRange(new DxRectangle(mapTransitionLocation.X, mapTransitionLocation.Y, 5, 5)).Any());
            levelEndSpawner.WithSpawnArea(new DxRectangle(mapTransitionLocation.X, mapTransitionLocation.Y, 1, 1));

            GameObject mapTransitionObject = GameObject.Builder().Build();

            LevelEndTrigger levelEndTrigger = new LevelEndTrigger(mapTransitionObject);
            DxGame.Instance.Model<EventModel>().AttachEventListener(new EventListener(levelEndTrigger.LevelEndListener));
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
            Validate.Hard.IsNotEmpty(maps_, $"Failed to find maps! Check {MAP_PATH} for valid descriptors");
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
    internal sealed class LevelEndTrigger
    {
        [DataMember] private readonly GameObject mapTransitionObject_;
        [DataMember] private bool alreadyTriggered_;

        [DataMember] private bool levelEndTriggered_;

        public void LevelEndListener(Event gameEvent)
        {
            Message message = gameEvent.Message;
            LevelEndRequest levelEndRequest = message as LevelEndRequest;
            if(ReferenceEquals(levelEndRequest, null))
            {
                return;
            }
            levelEndTriggered_ = true;
        }

        public LevelEndTrigger(GameObject mapTransitionObject)
        {
            Validate.Hard.IsNotNullOrDefault(mapTransitionObject,
                this.GetFormattedNullOrDefaultMessage("MapTransitionObject"));
            mapTransitionObject_ = mapTransitionObject;
            alreadyTriggered_ = false;
            levelEndTriggered_ = false;
        }

        public Tuple<bool, GameObject> CheckForLevelEndRequest(DxGameTime gameTime)
        {
            if(alreadyTriggered_)
            {
                return Tuple.Create<bool, GameObject>(false, null);
            }
            if(levelEndTriggered_)
            {
                alreadyTriggered_ = true;
                return Tuple.Create(true, mapTransitionObject_);
            }
            return Tuple.Create<bool, GameObject>(false, null);
        }
    }

    [DataContract]
    [Serializable]
    internal sealed class WaveNotifierTrigger
    {
        private readonly GameObject waveNotifier_;
        private bool alreadyActivated_;

        public WaveNotifierTrigger(GameObject waveNotifier)
        {
            Validate.Hard.IsNotNullOrDefault(waveNotifier, this.GetFormattedNullOrDefaultMessage(nameof(waveNotifier)));
            waveNotifier_ = waveNotifier;
            alreadyActivated_ = false;
        }

        public Tuple<bool, GameObject> CheckForInitialSpawn(DxGameTime gameTime)
        {
            if(alreadyActivated_)
            {
                return Tuple.Create<bool, GameObject>(false, null);
            }
            alreadyActivated_ = true;
            return Tuple.Create(true, waveNotifier_);
        }
    }
}