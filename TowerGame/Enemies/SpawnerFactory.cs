using System;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Primitives;
using DXGame.Main;
using DXGame.TowerGame.Components;
using NLog;

namespace DXGame.TowerGame.Enemies
{
    public static class SpawnerFactory
    {
        public static Spawner SimpleBoxSpawner()
        {
            var spawnFunction = new SimpleBoxSpawnFunction();
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
        private class SimpleBoxSpawnFunction
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
    }
}