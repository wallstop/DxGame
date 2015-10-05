using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Behavior;
using DXGame.Core.Primitives;
using DXGame.Main;

namespace DXGame.Core.Models
{
    /**
        The Sun Tzu of his time.

        <summary>
            Determines mappings of BehaviorComponent -> Goal
        </summary>
    */
    [Serializable]
    [DataContract]
    public class BehaviorModel : Model
    {
        //private Dictionary<BehaviorComponent, > 

        public BehaviorModel(DxGame game) 
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
