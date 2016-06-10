using System;
using DxCore.Core.Primitives;
using DXGame.Core;

namespace DxCore.Core.Lerp
{
    /**
        <summary>
            Describes some object that can lerp (https://en.wikipedia.org/wiki/Linear_interpolation).
            This is particularly useful in networked gamestates, where the current state of a thing has
            to be predicted.
        </summary>
    */

    internal interface IDxVectorLerpable : IIdentifiable
    {
        DxVector2 LerpValueSnapshot { get; }

        void Lerp(DxVector2 older, DxVector2 newer, TimeSpan oldTime, TimeSpan newTime, TimeSpan currentTime);
    }
}
