using System;
using System.Linq;
using Babel.Messaging;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Entity;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Events;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DXGame.Core;
using NLog;

namespace Babel.Enemies
{
    public static class SpawnerFactory
    {

        public static Spawner WaveBasedSmallBoxSpawner()
        {
            var spawnFunction = new SimpleSmallBoxWaveSpawnFunction();
            return RandomSpawner.Builder().WithSpawnTrigger(spawnFunction.Spawn).Build();
        }

        public static Spawner SimpleLargeBoxSpawner()
        {
            var spawnFunction = new SimpleLargeBoxSpawnFunction();
            return
                RandomSpawner.Builder()
                    .WithSpawnTrigger(spawnFunction.Spawn)
                    .Build();
        }

        public static Spawner SimpleSmallBoxSpawner()
        {
            var spawnFunction = new SimpleSmallBoxSpawnFunction();
            return
                RandomSpawner.Builder()
                    .WithSpawnTrigger(spawnFunction.Spawn)
                    .Build();
        }

        /*
            Instead of using a lambda, we create a special class to hold the state of our SimpleBox spawn dude.
            This way, we're able to serialize over the network. We could easily close over a boolean on the stack,
            but then we could not serialize the state.
        */

        [Serializable]
        private class SimpleLargeBoxSpawnFunction
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private TimeSpan lastSpawned_ = TimeSpan.Zero;

            private bool hasFullySpawned_ = false;

            private GameObject largeBox_ = null;
            
            public Tuple<bool, GameObject> Spawn(DxGameTime gameTime)
            {
                if (ReferenceEquals(largeBox_, null))
                {
                    largeBox_ = EnemyFactory.LargeBox();
                    return Tuple.Create(true, largeBox_);
                }
                if (!hasFullySpawned_)
                {
                    hasFullySpawned_ = true;
                    GameObject levelEndTrigger = EnemyFactory.LevelEndOnDeadListener(largeBox_);
                    return Tuple.Create(true, levelEndTrigger);
                }
                return Tuple.Create<bool, GameObject>(false, null);
            }
        }

        [Serializable]
        private class SimpleSmallBoxSpawnFunction
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private static readonly TimeSpan SPAWN_DELAY = TimeSpan.FromSeconds(1 / 10.0);
            private static readonly int MAX_BOXES_IN_PLAY = 25;
            private TimeSpan lastSpawned_ = TimeSpan.Zero;
            private int numSpawned_;

            private EntityType entityType_ = null;

            public Tuple<bool, GameObject> Spawn(DxGameTime gameTime)
            {
                if(ReferenceEquals(entityType_, null))
                {
                    GameObject smallBox = EnemyFactory.SmallBox();
                    EntityTypeComponent smallBoxTypeComponent = smallBox.ComponentOfType<EntityTypeComponent>();
                    entityType_ = smallBoxTypeComponent.EntityType;
                }
                var entityTypeComponents = DxGame.Instance.DxGameElements.OfType<EntityTypeComponent>();
                int numBoxesInPlay =
                    entityTypeComponents
                        .Count(entityTypeComponent => entityType_.Equals(entityTypeComponent.EntityType));

                var totalTime = gameTime.TotalGameTime;
                if(lastSpawned_ + SPAWN_DELAY < totalTime && numBoxesInPlay < MAX_BOXES_IN_PLAY)
                {
                    lastSpawned_ = totalTime;
                    ++numSpawned_;
                    LOG.Info($"Spawned {numSpawned_} boxes");
                    return Tuple.Create(true, EnemyFactory.SmallBox());
                }
                return Tuple.Create<bool, GameObject>(false, null);
            }
        }

        [Serializable]
        internal sealed class SimpleSmallBoxWaveSpawnFunction
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private static readonly int MAX_BOXES_IN_PLAY = 25;

            private static readonly int NUM_BOXES_PER_WAVE = 5;

            private int numBoxesSpawnedInWave_ = 0;
            private int totalBoxesSpawned_ = 0;

            private EventListener EventListener { get; }

            public SimpleSmallBoxWaveSpawnFunction()
            {
                EventModel eventModel = DxGame.Instance.Model<EventModel>();
                EventListener = new EventListener(ProcessEvent);
                eventModel.AttachEventListener(EventListener);
                numBoxesSpawnedInWave_ = 0;
            }

            private void ProcessEvent(Event gameEvent)
            {
                Message eventMessage = gameEvent.Message;
                NewWaveMessage maybeNewWaveMessage = eventMessage as NewWaveMessage;
                if(ReferenceEquals(maybeNewWaveMessage, null))
                {
                    return;
                }

                LOG.Info($"Received New Wave notification {maybeNewWaveMessage} - triggering spawn");
                numBoxesSpawnedInWave_ = 0;
            }

            public Tuple<bool, GameObject> Spawn(DxGameTime gameTime)
            {
                if(NUM_BOXES_PER_WAVE < numBoxesSpawnedInWave_ || MAX_BOXES_IN_PLAY < totalBoxesSpawned_)
                {
                    return Tuple.Create<bool, GameObject>(false, null);
                }

                ++numBoxesSpawnedInWave_;
                ++totalBoxesSpawned_;

                GameObject smallBox = EnemyFactory.SmallBox();
                return Tuple.Create(true, smallBox);
            }
        }
    }
}