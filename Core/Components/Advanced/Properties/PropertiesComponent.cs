using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Properties
{
    [Serializable]
    [DataContract]
    public abstract class PropertiesComponent : Component
    {
        protected PropertiesComponent(DxGame game)
            : base(game)
        {
        }
    }
}