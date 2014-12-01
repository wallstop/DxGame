using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXGame.Core.Components
{
    public abstract class DrawableComponent : Component
    {
        // TODO: This should have refs to something like a physics component (for position, size, etc)

        virtual public bool LoadContent()
        {
            // TODO
            return true;
        }

        virtual public bool Draw()
        {
            // TODO
            return true;
        }
    }
}
