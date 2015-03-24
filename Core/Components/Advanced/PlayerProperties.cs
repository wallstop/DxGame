using System;
using System.Runtime.Serialization;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    // TODO...
    [Serializable]
    [DataContract]
    public class PlayerProperties : PropertiesComponent
    {
        [DataMember]
        public int Health { get; set; }

        public PlayerProperties(DxGame game)
            : base(game)
        {
        }
    }
}