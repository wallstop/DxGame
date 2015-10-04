using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
        public BehaviorModel(DxGame game) 
            : base(game)
        {
        }


    }
}
