using DXGame.Core;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.TowerGame.Enemies
{

    public static class EnemyPropertyFactory
    {

        // TODO: Make this not shit
        public static EntityProperties PropertiesFor(EntityType entityType)
        {
            switch(entityType.Name)
            {
                case "SmallBox":
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
            moveSpeed: new Property<float>(7.5f, "MoveSpeed"),
            jumpSpeed: new Property<float>(18.0f, "JumpSpeed"),
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
