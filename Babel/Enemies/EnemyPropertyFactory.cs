using System;
using DxCore.Core;
using DxCore.Core.Properties;
using DXGame.Core;

namespace Babel.Enemies
{

    public static class EnemyPropertyFactory
    {

        // TODO: Make this not shit
        public static EntityProperties PropertiesFor(EntityType entityType)
        {
            switch(entityType.Name)
            {
                case "Sprites/SmallBox/SmallBox":
                    return SmallBoxProperties;
                case "LargeBox":
                    return LargeBoxProperties;
                case "Golem":
                    return GolemProperties;
                default:
                    throw new InvalidOperationException($"No known properties for {entityType.Name}");
            }
        }

        private static EntityProperties GolemProperties => new EntityProperties(
            health: new Property<int>(150, "Health"),
            maxHealth: new Property<int>(150, "MaxHealth"),
            defense: new Property<int>(1, "Defense"),
            moveSpeed: new Property<float>(0f, "MoveSpeed"),
            jumpSpeed: new Property<float>(0f, "JumpSpeed"),
            attackSpeed: new Property<int>(1, "AttackSpeed"));

        private static EntityProperties SmallBoxProperties => new EntityProperties(
            health: new Property<int>(5, "Health"),
            maxHealth: new Property<int>(5, "MaxHealth"),
            defense: new Property<int>(1, "Defense"),
            moveSpeed: new Property<float>(200f, "MoveSpeed"),
            jumpSpeed: new Property<float>(700f, "JumpSpeed"),
            attackSpeed: new Property<int>(1, "AttackSpeed"));

        private static EntityProperties LargeBoxProperties => new EntityProperties(
            health: new Property<int>(150, "Health"),
            maxHealth: new Property<int>(150, "MaxHealth"),
            defense: new Property<int>(1, "Defense"),
            moveSpeed: new Property<float>(2.5f, "MoveSpeed"),
            jumpSpeed: new Property<float>(5.0f, "JumpSpeed"),
            attackSpeed: new Property<int>(1, "AttackSpeed"));
    }
}
