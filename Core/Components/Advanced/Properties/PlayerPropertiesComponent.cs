using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Properties;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{
    [DataContract]
    [Serializable]
    public sealed class PlayerPropertiesComponent : Component
    {
        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        // TODO: Move all Properties to their actual types (AttackSpeed instead of double, Health instead of int, etc
        [DataMember]
        public Property<int> Health { get; private set; }

        [DataMember]
        public Property<int> MaxHealth { get; private set; }

        [DataMember]
        public Property<int> Defense { get; private set; }

        [DataMember]
        public Property<float> MoveSpeed { get; private set; }

        [DataMember]
        public Property<float> JumpSpeed { get; private set; }

        [DataMember]
        public Property<TimeSpan> AttackSpeed { get; private set; }

        public static PlayerPropertiesComponent DefaultPlayerProperties
        {
            get
            {
                return new PlayerPropertiesComponent(DxGame.Instance)
                {
                    // TODO: Tweak these values, they're ok-enough for now
                    Health = new Property<int>(10, "Health"),
                    MaxHealth = new Property<int>(10, "MaxHealth"),
                    Defense = new Property<int>(1, "Defense"),
                    MoveSpeed = new Property<float>(10.0f, "MoveSpeed"),
                    JumpSpeed = new Property<float>(8.0f, "JumpSpeed"),
                    AttackSpeed =
                        new Property<TimeSpan>(TimeSpan.FromMilliseconds(300), "AttackSpeed")
                };
            }
        }

        public PlayerPropertiesComponent(DxGame game)
            : base(game) { }
    }
}