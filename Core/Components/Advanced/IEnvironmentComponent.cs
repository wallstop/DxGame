using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Components.Advanced
{
    /* TODO: Refactor into Environment + Component interface (should probably rethink component heirarchy as well) */
    public interface IEnvironmentComponent
    {
        DxVector2 Position { get; }
        GameObject Parent { get;  }
    }
}
