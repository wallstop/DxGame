using DXGame.Core;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Damage;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Developer;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.TowerGame.Components;

namespace DXGame.TowerGame.Enemies
{
    public static class EnemyFactory
    {
        public static GameObject SmallBox()
        {
            string entityName = "SmallBox";
            var teamComponent = new TeamComponent(Team.EnemyTeam);
            // Build spatial component from bounds
            var enemySpatial = new MapBoundedSpatialComponent(DxVector2.EmptyVector, new DxVector2(50, 50));
            var platformDropper = new MapPlatformDropper();
            var entityType = EntityType.EntityTypeFor(entityName);
            var entityTypeComponent = new EntityTypeComponent(entityType);
            var pathfinding = new PathfindingInputComponent();
            var pathDrawer = new PathDrawer();
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
                        damageComponent, teamComponent, pathfinding, platformDropper, pathDrawer, entityTypeComponent)
                    .Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject, entityName);
            // Build and attach AI
            var simpleAi = SimpleEnemyAI.Builder().WithSpatialComponent(enemySpatial).Build();
            enemyObject.AttachComponent(simpleAi);

            return enemyObject;
        }

        public static GameObject LargeBox()
        {
            //string entityName = "LargeBox";
            TeamComponent teamComponent = new TeamComponent(Team.EnemyTeam);

            return null;

        }
    }
}
