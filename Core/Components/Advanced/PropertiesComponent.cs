using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class PropertiesComponent : Component
    {
        public PropertiesComponent(DxGame game)
            : base(game)
        {
        }
    }
}