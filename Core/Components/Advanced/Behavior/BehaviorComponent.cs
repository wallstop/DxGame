using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Behavior
{
    public class BehaviorComponent : Component
    {
        protected BehaviorComponent(DxGame game) 
            : base(game)
        {

        }


        public class BehaviorComponentBuilder : IBuilder<BehaviorComponent>
        {


            public BehaviorComponent Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}
