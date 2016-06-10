using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Models
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

        public BehaviorModel()
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
