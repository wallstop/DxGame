using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Properties;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{
    [DataContract]
    [Serializable]
    public class EntityPropertiesComponent : Component
    {
        /*
            TODO: Modify access of these properties. In general, we should leave it up to OTHER components to decide what to do with this information. 
            Properties classes should be a dump data store.
        */

        // TODO: Move all Properties to their actual types (AttackSpeed instead of double, Health instead of int, etc
        [DataMember]
        public Property<int> Health { get; protected set; }

        [DataMember]
        public Property<int> MaxHealth { get; protected set; }

        [DataMember]
        public Property<int> Defense { get; protected set; }

        [DataMember]
        public Property<float> MoveSpeed { get; protected set; }

        [DataMember]
        public Property<float> JumpSpeed { get; protected set; }

        [DataMember]
        public Property<TimeSpan> AttackSpeed { get; protected set; }

        public EntityPropertiesComponent(DxGame game)
            : base(game) { }
    }
}