using System.Collections.Generic;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Animation;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Damage;
using DXGame.Core.Components.Advanced.Impulse;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Developer;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.State;
using DXGame.Core.Utils;
using DXGame.Main;
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
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType));
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
            string entityName = "LargeBox";
            var teamComponent = new TeamComponent(Team.EnemyTeam);
            // Build spatial component from bounds
            var enemySpatial = new MapBoundedSpatialComponent(DxVector2.EmptyVector, new DxVector2(250, 250));
            var platformDropper = new MapPlatformDropper();
            var entityType = EntityType.EntityTypeFor(entityName);
            var entityTypeComponent = new EntityTypeComponent(entityType);
            var pathfinding = new PathfindingInputComponent();
            var enemyPhysics =
                MapCollidablePhysicsComponent.Builder().WithWorldForces().WithSpatialComponent(enemySpatial).Build();

            /* TODO: Configure properties */
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType));
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
                        damageComponent, teamComponent, pathfinding, platformDropper, entityTypeComponent)
                    .Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject, entityName);
            // Build and attach AI
            var simpleAi = SimpleEnemyAI.Builder().WithSpatialComponent(enemySpatial).Build();
            enemyObject.AttachComponent(simpleAi);

            return enemyObject;
        }

        public static GameObject LevelEndOnDeadListener(GameObject gameObject)
        {
            TriggerComponent trigger = new TriggerComponent(() =>
            {
                EventModel eventModel = DxGame.Instance.Model<EventModel>();
                if (ReferenceEquals(eventModel, null))
                {
                    return false;
                }
                EventRequest request = EventRequest.Builder().WithType<EntityDeathMessage>().Build();
                List<Event> deathEvents = eventModel.EventsFor(request, DxGame.Instance.CurrentTime);
                return
                    deathEvents.Select(deathEvent => deathEvent.Message as EntityDeathMessage)
                               .Any(deathMessage => Objects.Equals(gameObject, deathMessage.Entity));
            }, () =>
            {
                LevelEndRequest levelEndRequest = new LevelEndRequest();
                DxGame.Instance.BroadcastMessage(levelEndRequest);
            });
            GameObject triggerHolder = GameObject.Builder().WithComponent(trigger).Build();
            return triggerHolder;
        }
    }
}
