using System;
using System.Linq;
using DXGame.Core.Components.Advanced.Damage;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Main;
using DXGame.TowerGame.Components;
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
            var teamComponent = new TeamComponent(Team.EnemyTeam);
            // Build spatial component from bounds
            var enemySpatial =
                (BoundedSpatialComponent)
                    BoundedSpatialComponent.Builder().WithBounds(bounds).WithDimensions(new DxVector2(50, 50)).Build();
            var enemyPhysics =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(enemySpatial).Build();

            /* TODO: Configure properties */
            var enemyProperties = PlayerPropertiesComponent.DefaultPlayerProperties;
            var floatingHealthBar =
                FloatingHealthIndicator.Builder()
                    .WithEntityProperties(enemyProperties)
                    .WithPosition(enemySpatial)
                    .Build();

            var damageComponent = DamageComponent.Builder().WithEntityProprerties(enemyProperties).Build();

            var deathExploder = new DeathEffectComponent(DeathEffectComponent.SimpleEnemyBloodParticles);
            
            var enemyObject =
                GameObject.Builder()
                    .WithComponents(enemySpatial, enemyPhysics, enemyProperties, floatingHealthBar, deathExploder,
                        damageComponent, teamComponent)
                    .Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject,
                "SimpleBox");
            // Build and attach AI
            var simpleAi = SimpleEnemyAI.Builder().WithSpatialComponent(enemySpatial).Build();
            enemyObject.AttachComponent(simpleAi);

            return enemyObject;
        }

        public static Spawner SimpleBoxSpawner()
        {
            // Acquire a reference to the singleton instance of DxGame
            var game = DxGame.Instance;
            // Extract the map model and bounds
            var mapModel = game.Model<MapModel>();
            var spawnFunction = new SimpleBoxSpawnFunction();
            return
                Spawner.Builder()
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