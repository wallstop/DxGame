using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Models;
using DXGame.Core.Wrappers;
using DXGame.Main;
using DXGame.TowerGame.Components;
using DXGame.TowerGame.Enemies;

namespace DXGame.Core.Components.Advanced.Enemy
{
    public static class SpawnerFactory
    {
        public static Spawner SimpleBoxSpawner()
        {
            var game =  DxGame.Instance;
            var mapModel = game.Model<MapModel>();
            var bounds = mapModel.MapBounds;
            var enemySpatial = (BoundedSpatialComponent) new BoundedSpatialComponent(game).WithXMin(bounds.X)
                .WithXMax(bounds.Width)
                .WithXMin(bounds.Y)
                .WithYMax(bounds.Height)
                .WithDimensions(new DxVector2(50, 50)); // TODO: un-hard code these
            var enemyPhysics = new MapCollideablePhysicsComponent(game).WithSpatialComponent(enemySpatial);
            enemyPhysics.AddPostUpdater(
                    gameTime => WorldGravity.ApplyGravityToPhysics(gameTime, game, enemyPhysics));
            var animationBuilder = AnimationComponent.Builder().WithDxGame(game).WithPosition(enemySpatial);
            var enemyObject = GameObject.Builder().WithComponents(enemySpatial, enemyPhysics).Build();
            var stateMachine = EnemyFactory.SimpleBoxBehavior(game, animationBuilder, enemyObject);
            animationBuilder.WithStateMachine(stateMachine);
            enemyObject.AttachComponent(animationBuilder.Build());

            var simpleAi = new SimpleEnemyAI(game);
            enemyObject.AttachComponent(simpleAi);

            bool hasSpawned = false;
            SpawnTrigger spawnTrigger = gameTime => 
            {
                if (!hasSpawned)
                {
                    hasSpawned = true;
                    return Tuple.Create(hasSpawned, enemyObject);
                }
                return Tuple.Create<bool, GameObject>(false, null);
            };

            var spawnLocation = mapModel.PlayerSpawn;
            var spawnArea = new DxRectangle(spawnLocation, new DxVector2(50, 50));
            return Spawner.Builder().WithPosition(spawnLocation).WithGame(game).WithSpawnArea(spawnArea).WithSpawnTrigger(spawnTrigger).Build();
        }
    }
}
