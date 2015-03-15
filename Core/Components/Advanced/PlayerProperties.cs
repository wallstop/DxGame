using System;
using System.Runtime.Serialization;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class PlayerProperties : PropertiesComponent
    {
        public int Health { get; set; }

        public PlayerProperties(DxGame game)
            : base(game)
        {
        }
    }
}