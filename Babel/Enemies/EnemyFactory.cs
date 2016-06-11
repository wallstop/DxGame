using System.Collections.Generic;
using System.Linq;
using Babel.Components;
using Babel.Items;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Damage;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Impulse;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Advanced.Triggers;
using DxCore.Core.Components.Developer;
using DxCore.Core.Events;
using DxCore.Core.Experience;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.State;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace Babel.Enemies
{
    public static class EnemyFactory
    {
        /* TODO: Remove / refactor */

        private static List<GameObject> Spawn<T>(DxVector2 position) where T : ItemComponent
        {
            GameObject item = ItemFactory.GenerateVisible<T>(position);
            List<GameObject> spawnedObjects = new List<GameObject> { item };
            return spawnedObjects;
        }

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
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType),
                EntityPropertiesComponent.NullLevelUpResponse);
            ExperienceDropperComponent experienceDropper = new ExperienceDropperComponent(new Experience(50));

            ItemDropperComponent itemDropper = new ItemDropperComponent(1.0, Spawn<PandorasBox>);
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
                        damageComponent, teamComponent, pathfinding, platformDropper, pathDrawer, entityTypeComponent, experienceDropper, itemDropper)
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
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType), EntityPropertiesComponent.NullLevelUpResponse);
            ExperienceDropperComponent experienceDropper = new ExperienceDropperComponent(new Experience(50));
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
                        damageComponent, teamComponent, pathfinding, platformDropper, entityTypeComponent, experienceDropper)
                    .Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject, entityName);
            // Build and attach AI
            var simpleAi = SimpleEnemyAI.Builder().WithSpatialComponent(enemySpatial).Build();
            enemyObject.AttachComponent(simpleAi);

            return enemyObject;
        }

        public static GameObject Golem()
        {
            string entityName = "Golem";
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
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType), EntityPropertiesComponent.NullLevelUpResponse);
            ExperienceDropperComponent experienceDropper = new ExperienceDropperComponent(new Experience(50));
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
                        damageComponent, teamComponent, pathfinding, platformDropper, entityTypeComponent, experienceDropper)
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
                levelEndRequest.Emit();
            });
            GameObject triggerHolder = GameObject.Builder().WithComponent(trigger).Build();
            return triggerHolder;
        }
    }
}
