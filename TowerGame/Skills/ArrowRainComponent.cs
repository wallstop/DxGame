using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.TowerGame.Skills
{
    /* TODO: We need to launch the arrow away from the player & have it draw it's own boxes & do damage over time to entity's within it's bounding boxes */
    [Serializable]
    [DataContract]
    public class ArrowRainComponent : Component
    {
        public ArrowRainComponent(DxGame game) 
            : base(game)
        {
            // blahhh todoooooo
        }
    }
}
