using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Core.Components
{
    public abstract class UpdateableComponent : Component
    {
        virtual public bool Update()
        {
            return true;
        }
    }
}
