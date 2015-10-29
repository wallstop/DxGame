using DXGame.Core.Properties;
using System;

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
            attackSpeed: new Property<TimeSpan>(TimeSpan.FromMilliseconds(300), "AttackSpeed"));
    }
}