﻿using DxCore.Core.Primitives;
using DXGame.Core;

namespace DxCore.Core.Components.Advanced
{
    /* TODO: Refactor into Environment + Component interface (should probably rethink component heirarchy as well) */

    public interface IEnvironmentComponent
    {
        DxVector2 Position { get; }
        GameObject Parent { get; }
    }
}
