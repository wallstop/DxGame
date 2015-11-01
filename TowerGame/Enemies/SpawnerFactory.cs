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
            private static readonly TimeSpan SPAWN_DELAY = TimeSpan.FromSeconds(1 / 10.0);
            private static readonly int MAX_BOXES_IN_PLAY = 15;
            private TimeSpan lastSpawned_ = TimeSpan.Zero;
            private int numSpawned_;
            
            public Tuple<bool, GameObject> Spawn(DxGameTime gameTime)
            {
                var numBoxesInPlay = DxGame.Instance.DxGameElements.OfType<SimpleEnemyAI>().Count();

                var totalTime = gameTime.TotalGameTime;
                if (lastSpawned_ + SPAWN_DELAY < totalTime && numBoxesInPlay < MAX_BOXES_IN_PLAY)
                {
                    lastSpawned_ = totalTime;
                    ++numSpawned_;
                    LOG.Info($"Spawned {numSpawned_} boxes");
                    return Tuple.Create(true, EnemyFactory.SmallBox());
                }
                return Tuple.Create<bool, GameObject>(false, null);
            }
        }
    }
}