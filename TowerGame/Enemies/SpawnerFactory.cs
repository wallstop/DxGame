using System;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using DXGame.TowerGame.Components;
using NLog;

namespace DXGame.TowerGame.Enemies
{
    public static class SpawnerFactory
    {
        public static Spawner WaveCounterSpawner()
        {
            // TODO
            return null;
        }

        public static Spawner WaveBasedSmallBoxSpawner()
        {
            // TODO
            return null;
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
        private class SimpleSmallBoxWaveSpawnFunction
        {
            private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
            private static readonly TimeSpan WAVE_DELAY = TimeSpan.FromSeconds(30.0);


        }
    }
}