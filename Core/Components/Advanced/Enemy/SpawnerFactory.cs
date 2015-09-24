using System;
using System.Linq;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using DXGame.TowerGame.Components;
using DXGame.TowerGame.Enemies;
using NLog;

namespace DXGame.Core.Components.Advanced.Enemy
{
    public static class SpawnerFactory
    {
        private static GameObject SimpleBoxSpawnLogic()
        {
            // Acquire a reference to the singleton instance of DxGame
            var game = DxGame.Instance;
            // Extract the map model and bounds
            var mapModel = game.Model<MapModel>();
            var bounds = mapModel.MapBounds;
            // Build spatial component from bounds
            var enemySpatial = (BoundedSpatialComponent) new BoundedSpatialComponent(game).WithXMin(bounds.X)
                .WithXMax(bounds.Width)
                .WithXMin(bounds.Y)
                .WithYMax(bounds.Height)
                .WithDimensions(new DxVector2(50, 50)); // TODO: un-hard code these
            var enemyPhysics =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(enemySpatial).Build();
            var animationBuilder = AnimationComponent.Builder().WithDxGame(game).WithPosition(enemySpatial);
            var enemyObject = GameObject.Builder().WithComponents(enemySpatial, enemyPhysics).Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            var stateMachine = EnemyFactory.SimpleBoxBehavior(game, animationBuilder, enemyObject);
            // Incremental state update to the animation builder
            animationBuilder.WithStateMachine(stateMachine);
            enemyObject.AttachComponent(animationBuilder.Build());
            // Build and attach AI
            var simpleAi = SimpleEnemyAI.Builder().WithDxGame(game).WithSpatialComponent(enemySpatial).Build();
            enemyObject.AttachComponent(simpleAi);
            enemyObject.AttachComponent(stateMachine);

            return enemyObject;
        }

        public static Spawner SimpleBoxSpawner()
        {
            // Acquire a reference to the singleton instance of DxGame
            var game = DxGame.Instance;
            // Extract the map model and bounds
            var mapModel = game.Model<MapModel>();
            var spawnFunction = new SimpleBoxSpawnFunction();

            var spawnLocation = mapModel.PlayerSpawn;
            var spawnArea = new DxRectangle(spawnLocation, new DxVector2(50, 50));
            return
                Spawner.Builder()
                    .WithPosition(spawnLocation)
                    .WithGame(game)
                    .WithSpawnArea(spawnArea)
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
            private static readonly int MAX_BOXES_IN_PLAY = 10;
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
                    return Tuple.Create(true, SimpleBoxSpawnLogic());
                }
                return Tuple.Create<bool, GameObject>(false, null);
            }
        }
    }
}