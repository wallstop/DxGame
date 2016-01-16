using DXGame.Core.Properties;
using System;
using DXGame.Core.Messaging;

namespace DXGame.TowerGame.Player
{
    public static class PlayerFactory
    {
        public static EntityProperties BasicPlayerProperties => new EntityProperties(
            health: new Property<int>(10, "Health"),
            maxHealth: new Property<int>(10, "MaxHealth"),
            defense: new Property<int>(1, "Defense"),
            moveSpeed: new Property<float>(6.5f, "MoveSpeed"),
            jumpSpeed: new Property<float>(13.0f, "JumpSpeed"),
            attackSpeed: new Property<int>(10, "AttackSpeed"),
            attackDamage: new Property<int>(5, "AttackDamage"));

        public static void GenericLevelUp(EntityProperties properties, LeveledUpMessage levelUpMessage)
        {

            const int healthGain = 4;
            properties.MaxHealth.BaseValue += healthGain;

            const int attackSpeedGain = 5;
            properties.AttackSpeed.BaseValue += attackSpeedGain;

            const int attackDamageGain = 2;
            properties.AttackDamage.BaseValue += attackDamageGain;

            /* Only increase defense if the new level is a multiple of 3 */
            if(levelUpMessage.NewLevel % 3 == 0)
            {
                const int defenseGain = 1;
                properties.Defense.BaseValue += defenseGain;
            }

        }
    }
}