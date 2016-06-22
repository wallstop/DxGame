using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Triggers;
using DxCore.Core.Components.Basic;
using DxCore.Core.Events;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.Level
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
            foreach(Spawner spawner in Spawners)
            {
                spawner.Create();
            }

            entityManager_.Create();

            EventObserver spawnObserver =
                EventObserver.EventObserverBuilder()
                    .WithAcceptance(DetermineLevelSpawnEvent)
                    .WithAction(AddEntityToManagementPool)
                    .Build();
            entityManager_.Manage(spawnObserver);

            spawnObserver.Create();
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

        // TODO: Manage EntityDeath (not just entity spawn) 
        private void AddEntityToManagementPool(Event gameEvent)
        {
            EntitySpawnedMessage spawnMessage = gameEvent.Message as EntitySpawnedMessage;
            Validate.Hard.IsNotNullOrDefault(spawnMessage,
                $"Expected the Event to contain an {typeof(EntitySpawnedMessage)}, but it was not. How did this pass {nameof(DetermineLevelSpawnEvent)}?");
            Component spawnedComponent;
            if(spawnMessage.TryGetSpawnedEntity(out spawnedComponent))
            {
                entityManager_.Manage(spawnedComponent);
                return;
            }

            GameObject spawnedGameObject;
            if(spawnMessage.TryGetSpawnedEntity(out spawnedGameObject))
            {
                spawnedGameObject.Components.ToList().ForEach(component => entityManager_.Manage(component));
            }
        }

        public override void Remove()
        {
            entityManager_.Remove();
            Spawners.ForEach(spawner => spawner.Remove());
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
                Validate.Hard.IsNotNull(map_, this.GetFormattedNullOrDefaultMessage(map_));
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
                Validate.Hard.IsNotNull(spawner);
                spawners_.Add(spawner);
                return this;
            }
        }
    }
}