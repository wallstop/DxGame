using System;
using System.Runtime.Serialization;
using DXGame.Core.Properties;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{

    /*
        TODO: Turn this into a factory somewhere. That way simply go to some kind of "EntityPropertiesFactory" to get properties for all kinds of named types
    */
    [Serializable]
    [DataContract]
    public class PlayerPropertiesComponent : EntityPropertiesComponent
    {
        public static PlayerPropertiesComponent DefaultPlayerProperties
            => new PlayerPropertiesComponent(DxGame.Instance)
            {
                // TODO: Tweak these values, they're ok-enough for now
                Health = new Property<int>(10, "Health"),
                MaxHealth = new Property<int>(10, "MaxHealth"),
                Defense = new Property<int>(1, "Defense"),
                MoveSpeed = new Property<float>(6.5f, "MoveSpeed"),
                JumpSpeed = new Property<float>(13.0f, "JumpSpeed"),
                AttackSpeed =
                    new Property<TimeSpan>(TimeSpan.FromMilliseconds(300), "AttackSpeed")
            };

        public PlayerPropertiesComponent(DxGame game)
            : base(game)
        {
        }
    }
}