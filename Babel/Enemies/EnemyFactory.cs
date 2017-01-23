using System.Collections.Generic;
using Babel.Components;
using Babel.Items;
using DxCore.Core;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Advanced.Behaviors;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Damage;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Advanced.Impulse;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Advanced.Team;
using DxCore.Core.Components.Advanced.Entity;
using DxCore.Core.Components.Advanced.Player;
using DxCore.Core.Experience;
using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using DxCore.Core.State;
using DxCore.Core.Messaging;
using DXGame.Core.Behaviors;

namespace Babel.Enemies
{
    public static class EnemyFactory
    {
        /* TODO: Remove / refactor */

        private static List<GameObject> Spawn<T>(DxVector2 position) where T : ItemComponent
        {
            GameObject item = ItemFactory.GenerateVisible<T>(position);
            List<GameObject> spawnedObjects = new List<GameObject> {item};
            return spawnedObjects;
        }

        public static GameObject SmallBox()
        {
            string entityName = "Sprites/SmallBox/SmallBox";
            var teamComponent = new TeamComponent(Team.EnemyTeam);
            // Build spatial component from bounds
            var platformDropper = new MapPlatformDropper();
            var entityType = EntityType.EntityTypeFor(entityName);
            var entityTypeComponent = new EntityTypeComponent(entityType);
            var pathfinding = new PathfindingInputComponent();
            var enemyPhysics =
                PhysicsComponent.Builder()
                    .WithBounds(new DxVector2(50, 50))
                    .WithCollisionGroup(CollisionGroup.Entities)
                    .WithCollidesWith(CollisionGroup.All.Not(CollisionGroup.Entities))
                    .WithPosition(DxVector2.EmptyVector)
                    .WithDirectPositionAccess()
                    .WithWorldCollisionSensor()
                    .Build();

            /* TODO: Configure properties */
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType),
                EntityPropertiesComponent.NullLevelUpResponse);
            ExperienceDropperComponent experienceDropper = new ExperienceDropperComponent(new Experience(50));

            ItemDropperComponent itemDropper = new ItemDropperComponent(1.0, Spawn<PandorasBox>);
            var floatingHealthBar =
                FloatingHealthIndicator.Builder()
                    .WithEntityProperties(enemyProperties)
                    .WithPosition(enemyPhysics)
                    .Build();

            var damageComponent = DamageComponent.Builder().WithEntityProprerties(enemyProperties).Build();

            var deathExploder = new DeathEffectComponent(DeathEffectComponent.SimpleEnemyBloodParticles);

            var affinityComponent = AffinityComponent.Builder().WithAffinity(Commandment.Movement, Attribute.Speed, 0.75f).Build();

            var behaviorComponent = new BehaviorComponent();

            var enemyObject =
                GameObject.Builder()
                    .WithComponents(enemyPhysics, enemyProperties, floatingHealthBar, deathExploder, damageComponent,
                        teamComponent, pathfinding, platformDropper, entityTypeComponent, experienceDropper, 
                        itemDropper, affinityComponent, behaviorComponent)
                    .Build();
            // Create a state machine for the enemy in question
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject, entityName);

            return enemyObject;
        }

        public static GameObject LargeBox()
        {
            string entityName = "LargeBox";
            var teamComponent = new TeamComponent(Team.EnemyTeam);
            // Build spatial component from bounds
            var platformDropper = new MapPlatformDropper();
            var entityType = EntityType.EntityTypeFor(entityName);
            var entityTypeComponent = new EntityTypeComponent(entityType);
            var pathfinding = new PathfindingInputComponent();
            var enemyPhysics =
                /* Need direct positional access so spawner can spawn these guys properly. TODO: Fix */
                PhysicsComponent.Builder()
                    .WithBounds(new DxVector2(250, 250))
                    .WithPosition(DxVector2.EmptyVector)
                    .WithDirectPositionAccess()
                    .Build();

            /* TODO: Configure properties */
            var enemyProperties = new EntityPropertiesComponent(EnemyPropertyFactory.PropertiesFor(entityType),
                EntityPropertiesComponent.NullLevelUpResponse);
            ExperienceDropperComponent experienceDropper = new ExperienceDropperComponent(new Experience(50));
            var floatingHealthBar =
                FloatingHealthIndicator.Builder()
                    .WithEntityProperties(enemyProperties)
                    .WithPosition(enemyPhysics)
                    .Build();

            var damageComponent = DamageComponent.Builder().WithEntityProprerties(enemyProperties).Build();

            var deathExploder = new DeathEffectComponent(DeathEffectComponent.SimpleEnemyBloodParticles);

            var affinityComponent = AffinityComponent.Builder().WithAffinity(Commandment.Movement, Attribute.Speed, 0.25f).Build();

            var behaviorComponent = new BehaviorComponent();

            var enemyObject =
                GameObject.Builder()
                    .WithComponents(enemyPhysics, enemyProperties, floatingHealthBar, deathExploder, damageComponent,
                        teamComponent, pathfinding, platformDropper, entityTypeComponent, experienceDropper,
                        affinityComponent, behaviorComponent)
                    .Build();
            // Create a state machine for the enemy in question
            // Horrifically complex; weep, gnash teeth
            StateMachineFactory.BuildAndAttachBasicMovementStateMachineAndAnimations(enemyObject, entityName);

            return enemyObject;
        }
    }
}
