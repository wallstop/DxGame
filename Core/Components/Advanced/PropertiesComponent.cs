using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Microsoft.Xna.Framework;

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