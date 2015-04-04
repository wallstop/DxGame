using System;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;
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
        public DxVector2 MoveSpeed { get; set; }

        [DataMember]
        public float JumpSpeed { get; set; }

        [DataMember]
        public float AttackSpeed { get; set; }

        public PlayerPropertiesComponent(DxGame game)
            : base(game) { }
    }
}