using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using DXGame.TowerGame.Components;
using DXGame.TowerGame.Enemies;

namespace DXGame.Core.Components.Advanced.Enemy
{
    public static class SpawnerFactory
    {
        /*
            Instead of using a lambda, we create a special class to hold the state of our SimpleBox spawn dude.
            This way, we're able to serialize over the network. We could easily close over a boolean on the stack,
            but then we could not serialize the state.
        */
        [Serializable]
        private class SimpleBoxSpawnFunction
        {
            private bool hasSpawned_ = false;
            private GameObject enemyObject_;

            public SimpleBoxSpawnFunction(GameObject enemyObject)
            {
                enemyObject_ = enemyObject;
            }

            public Tuple<bool, GameObject> Spawn(DxGameTime gameTime)
            {
                if (!hasSpawned_)
                {
                    hasSpawned_ = true;
                    return Tuple.Create(hasSpawned_, enemyObject_);
                }
                return Tuple.Create<bool, GameObject>(false, null);
            }
        }

        public static Spawner SimpleBoxSpawner()
        {
            // Acquire a reference to the singleton instance of DxGame
            var game =  DxGame.Instance;
            // Extract the map model and bounds
            var mapModel = game.Model<MapModel>();
            var bounds = mapModel.MapBounds;
            // Build spatial component from bounds
            var enemySpatial = (BoundedSpatialComponent) new BoundedSpatialComponent(game).WithXMin(bounds.X)
                .WithXMax(bounds.Width)
                .WithXMin(bounds.Y)
                .WithYMax(bounds.Height)
                .WithDimensions(new DxVector2(50, 50)); // TODO: un-hard code these
            var enemyPhysics = new MapCollideablePhysicsComponent(game).WithSpatialComponent(enemySpatial);
            GravityApplier.ApplyGravityToPhysics(enemyPhysics);
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

            var spawnFunction = new SimpleBoxSpawnFunction(enemyObject);

            var spawnLocation = mapModel.PlayerSpawn;
            var spawnArea = new DxRectangle(spawnLocation, new DxVector2(50, 50));
            return Spawner.Builder().WithPosition(spawnLocation).WithGame(game).WithSpawnArea(spawnArea).WithSpawnTrigger(spawnFunction.Spawn).Build();
        }
    }
}
