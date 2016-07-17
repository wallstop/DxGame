using System;
using System.Linq;
using Babel.Messaging;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
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
            throw new NotImplementedException();
        }

        public static Spawner SimpleSmallBoxSpawner()
        {
            var spawnFunction = new SimpleSmallBoxSpawnFunction();
            return RandomSpawner.Builder().WithSpawnTrigger(spawnFunction.Spawn).Build();
        }

        /*
            Instead of using a lambda, we create a special class to hold the state of our SimpleBox spawn dude.
            This way, we're able to serialize over the network. We could easily close over a boolean on the stack,
            but then we could not serialize the state.
        */

        [Serializable]
        private class SimpleSmallBoxSpawnFunction
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private static readonly TimeSpan SPAWN_DELAY = TimeSpan.FromSeconds(1 / 10.0);
            private static readonly int MAX_BOXES_IN_PLAY = 25;
            private TimeSpan lastSpawned_ = TimeSpan.Zero;
            private int numSpawned_;

            private EntityType entityType_;

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
                    entityTypeComponents.Count(entityTypeComponent => entityType_.Equals(entityTypeComponent.EntityType));

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
        internal sealed class SimpleSmallBoxWaveSpawnFunction : IDisposable
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private static readonly int MAX_BOXES_IN_PLAY = 50;

            private static readonly int NUM_BOXES_PER_WAVE = 5;

            private int numBoxesSpawnedInWave_;
            private int totalBoxesSpawned_;

            private UniqueId Id { get; }
            private MessageHandler MessageHandler { get; }

            public SimpleSmallBoxWaveSpawnFunction()
            {
                Id = new UniqueId();
                MessageHandler = new MessageHandler(Id);
                MessageHandler.RegisterMessageHandler<NewWaveMessage>(HandleNewWaveMessage);
                numBoxesSpawnedInWave_ = 0;
            }

            private void HandleNewWaveMessage(NewWaveMessage maybeNewWaveMessage)
            {
                LOG.Info("Received New Wave notification {0} - triggering spawn", maybeNewWaveMessage);
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

            public void Dispose()
            {
                MessageHandler.Deregister();
            }
        }
    }
}