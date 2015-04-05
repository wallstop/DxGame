using System;
using System.Runtime.Serialization;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{
    [DataContract]
    [Serializable]
    public class PlayerPropertiesComponent : PropertiesComponent
    {
        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        [DataMember]
        public int Health { get; set; }

        [DataMember]
        public int MaxHealth { get; set; }

        [DataMember]
        public int Defense { get; set; }

        [DataMember]
        public float MoveSpeed { get; set; }

        [DataMember]
        public float JumpSpeed { get; set; }

        [DataMember]
        public TimeSpan AttackSpeed { get; set; }

        public static PlayerPropertiesComponent DefaultPlayerProperties
        {
            get
            {
                return new PlayerPropertiesComponent(DxGame.Instance)
                {
                    Health = 10,
                    MaxHealth = 10,
                    Defense = 1,
                    MoveSpeed = 10.0f,
                    JumpSpeed = 8.0f,
                    AttackSpeed = TimeSpan.FromMilliseconds(300)
                };
            }
        }

        public PlayerPropertiesComponent(DxGame game)
            : base(game) { }
    }
}