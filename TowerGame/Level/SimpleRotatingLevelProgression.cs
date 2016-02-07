using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Level;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Physics;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using DXGame.TowerGame.Components;
using DXGame.TowerGame.Components.Waves;
using DXGame.TowerGame.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private int currentMapIndex_ = -1;

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
            if(currentMapIndex_ == maps_.Count)
            {
                return GenerateLastLevel();
            }
            ++currentMapIndex_;


            Spawner levelEndSpawner = GenerateMapTransitionSpawner();
            Spawner waveBasedBoxSpawner = SpawnerFactory.WaveBasedSmallBoxSpawner();
            Spawner waveNotifierSpawner = GenerateWaveChangeNotifierSpawner();
            Spawner waveEmitterSpawner = GenerateWaveEmitterSpawner();

            Core.Level.Level generatedLevel =
                Core.Level.Level.Builder()
                    .WithMap(CurrentMap)
                    .WithSpawners(levelEndSpawner, waveNotifierSpawner, waveEmitterSpawner, waveBasedBoxSpawner)
                    .Build();

            return generatedLevel;
        }

        private Core.Level.Level GenerateLastLevel()
        {
            SpriteFont comicSans = DxGame.Instance.Content.Load<SpriteFont>("Fonts/ComicSans");
            /* Force to be spatial so we can abuse PhysicsComponents */
            SpatialComponent space = new MapBoundedSpatialComponent(DxVector2.EmptyVector, new DxVector2(200, 20));
            TextComponent winningText = new TextComponent(space, comicSans, "Fonts/ComicSans")
            {
                Text = "WOW YOU WON GOOD JOB",
                DxColor = new DxColor(Color.Pink)
            };

            RandomTextMover randomTextMover = new RandomTextMover();

            Force textMovingForce = new Force(DxVector2.EmptyVector, DxVector2.EmptyVector, randomTextMover.DissipationFunction, "Random Text Mover");

            PhysicsComponent textPhysics = MapCollidablePhysicsComponent.Builder().WithSpatialComponent(space).WithForce(textMovingForce).Build();

            GameObject bouncingStupidText = GameObject.Builder().WithComponents(space, winningText, textPhysics).Build();

            DxRectangle mapSize = new DxRectangle(0, 0, 1280, 720);

            MapDescriptor winningMapDescriptor = MapDescriptor.Builder().WithSize(mapSize).Build();

            Map winningMap = new Map(winningMapDescriptor);
            winningMap.LoadContent();
            winningMap.Initialize();

            Spawner textSpawner = new RandomSpawner(mapSize, Spawner.ImmediateSpawnTriggerFor(bouncingStupidText));

            Core.Level.Level winningLevel =
                Core.Level.Level.Builder().WithMap(winningMap).WithSpawner(textSpawner).Build();

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

    [DataContract]
    [Serializable]
    internal sealed class RandomTextMover
    {
        private static readonly TimeSpan DIRECTION_CHANGE_DELAY = TimeSpan.FromSeconds(1);
        private static readonly float FORCE = 2.0f;

        private TimeSpan lastChanged_;

        private DxVector2 currentAcceleration_;

        public Tuple<bool, DxVector2> DissipationFunction(DxVector2 externalVelocity, DxVector2 currentAcceleration,
            DxGameTime gameTime)
        {
            if(gameTime.TotalGameTime >= lastChanged_ + DIRECTION_CHANGE_DELAY)
            {
                lastChanged_ = gameTime.TotalGameTime;
                DxRadian targetDirection;
                do
                {
                    targetDirection = new DxRadian(ThreadLocalRandom.Current.NextDouble(0, 2 * Math.PI));
                } while(targetDirection.Value - currentAcceleration_.Radian.Value < Math.PI / 2);
                currentAcceleration_ = targetDirection.UnitVector * FORCE;
            }
            bool targetXPositive = currentAcceleration_.X > 0;
            bool targetYPositive = currentAcceleration_.Y > 0;

            /* 
                What we want is the velocity to pretty much be whatever our currentAcceleration_ value is.
                While I'm sure there's all kinds of math to do this properly, I'm not going to look it up.
                Instead, I'm going to just "call it good" if we're "on track" towards our target x values and 
                y values.
            */
            DxVector2 accelerationResult = currentAcceleration_;

            if(targetXPositive)
            {
                if(externalVelocity.X > accelerationResult.X)
                {
                    accelerationResult.X = 0;
                }
            }
            else // x is negative
            {
                if(externalVelocity.X < accelerationResult.X)
                {
                    accelerationResult.X = 0;
                }
            }
            if(targetYPositive)
            {
                if(externalVelocity.Y > accelerationResult.Y)
                {
                    accelerationResult.Y = 0;
                }
            }
            else // y is negative
            {
                if(externalVelocity.Y < accelerationResult.Y)
                {
                    accelerationResult.Y = 0;
                }
            }
            return Tuple.Create<bool, DxVector2>(false, accelerationResult);
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

    [DataContract]
    [Serializable]
    internal sealed class WaveNotifierTrigger
    {
        private readonly GameObject waveNotifier_;
        private bool alreadyActivated_;

        public WaveNotifierTrigger(GameObject waveNotifier)
        {
            Validate.IsNotNullOrDefault(waveNotifier, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(waveNotifier)));
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
            return Tuple.Create<bool, GameObject>(true, waveNotifier_);
        }
    }
}