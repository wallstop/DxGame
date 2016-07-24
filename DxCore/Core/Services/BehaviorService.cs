using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Services
{
    /**
        The Sun Tzu of his time.

        <summary>
            Determines mappings of BehaviorComponent -> Goal
        </summary>
    */
    [Serializable]
    [DataContract]
    public class BehaviorService : Service
    {
        //private Dictionary<BehaviorComponent, > 

        public BehaviorService()
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
