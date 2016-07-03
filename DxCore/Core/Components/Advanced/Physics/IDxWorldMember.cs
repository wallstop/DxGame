﻿using DxCore.Core.Physics;
using DxCore.Core.Primitives;
using FarseerPhysics.Dynamics;

namespace DxCore.Core.Components.Advanced.Physics
{
    public interface IDxWorldMember
    {
        DxVector2 Position { get; }

        float Height { get; }

        float Width { get; }

        DxRectangle Space { get; }

        DxVector2 Center { get; }

        Fixture Fixture { get; }

        PhysicsType PhysicsType { get; }
    }
}
