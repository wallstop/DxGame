using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Components.Advanced
{
    public interface IEnvironmentComponent
    {
        DxVector2 Position { get; }
        GameObject Parent { get;  }
    }
}
