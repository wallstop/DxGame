using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Advanced.Triggers;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.Messaging;
using DXGame.Core.Messaging.Entity;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Level
{
    [DataContract]
    [Serializable]
    public class Level : Component
    {
        [DataMember] private readonly ManagerComponent entityManager_ = new ManagerComponent();

        [DataMember]
        public Map.Map Map { get; }

        [DataMember]
        public TimeSpan LevelTime { get; private set; }

        [DataMember]
        public List<Spawner> Spawners { get; }

        private Level(Map.Map map, IEnumerable<Spawner> spawners)
        {
            Map = map;
            Spawners = spawners.ToList();
            LevelTime = TimeSpan.Zero;
        }

        public override void Initialize()
        {
            Spawners.ForEach(spawner => DxGame.Instance.AddAndInitializeComponent(spawner));
            DxGame.Instance.AddAndInitializeComponent(entityManager_);
            EventObserver spawnObserver =
                EventObserver.EventObserverBuilder()
                    .WithAcceptance(DetermineLevelSpawnEvent)
                    .WithAction(AddEntityToManagementPool)
                    .Build();
            entityManager_.Manage(spawnObserver);
            DxGame.Instance.AddAndInitializeComponent(spawnObserver);
            base.Initialize();
        }

        protected override void Update(DxGameTime gameTime)
        {
            LevelTime += gameTime.ElapsedGameTime;
        }

        private bool DetermineLevelSpawnEvent(Event gameEvent)
        {
            Message gameMessage = gameEvent.Message;
            EntitySpawnedMessage spawnMessage = gameMessage as EntitySpawnedMessage;
            if(!ReferenceEquals(spawnMessage, null))
            {
                return Spawners.Any(spawner => Equals(spawner.Id, spawnMessage.SpawnerId));
            }
            return false;
        }

        private void AddEntityToManagementPool(Event gameEvent)
        {
            EntitySpawnedMessage spawnMessage = gameEvent.Message as EntitySpawnedMessage;
            Validate.IsNotNullOrDefault(spawnMessage,
                $"Expected the Event to contain an {typeof(EntitySpawnedMessage)}, but it was not. How did this pass {nameof(DetermineLevelSpawnEvent)}?");
            if(spawnMessage.SpawnedComponent.HasValue)
            {
                Component spawnedComponent;
                bool stillExists = spawnMessage.SpawnedComponent.Value.TryGetTarget(out spawnedComponent);
                if(stillExists)
                {
                    entityManager_.Manage(spawnedComponent);
                }
            }
            else if(spawnMessage.SpawnedObject.HasValue)
            {
                GameObject spawnedObject;
                bool stillExists = spawnMessage.SpawnedObject.Value.TryGetTarget(out spawnedObject);
                if(stillExists)
                {
                    spawnedObject.Components.ToList().ForEach(component => entityManager_.Manage(component));
                }
            }
        }

        public override void Dispose()
        {
            entityManager_.Dispose();
            Spawners.ForEach(spawner => spawner.Dispose());
        }

        public static LevelBuilder Builder()
        {
            return new LevelBuilder();
        }

        public class LevelBuilder : IBuilder<Level>
        {
            private readonly HashSet<Spawner> spawners_ = new HashSet<Spawner>();
            private Map.Map map_;

            public Level Build()
            {
                Validate.IsNotNull(map_, StringUtils.GetFormattedNullOrDefaultMessage(this, map_));
                return new Level(map_, spawners_);
            }

            public LevelBuilder WithMap(Map.Map map)
            {
                map_ = map;
                return this;
            }

            public LevelBuilder WithSpawners(params Spawner[] spawners)
            {
                foreach(Spawner spawner in spawners)
                {
                    WithSpawner(spawner);
                }
                return this;
            }

            public LevelBuilder WithSpawner(Spawner spawner)
            {
                Validate.IsNotNull(spawner);
                spawners_.Add(spawner);
                return this;
            }
        }
    }
}