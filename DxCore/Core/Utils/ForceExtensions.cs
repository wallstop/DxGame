using DxCore.Core.Primitives;
using DxCore.Core.Services;

namespace DxCore.Core.Utils
{
    public static class ForceExtensions
    {
        public static DxVector2 FarseerScaled(this DxVector2 force)
        {
            return force / DxGame.Instance.PhysicsUpdateFrequency * WorldService.DxToFarseerScale;
        }
    }
}
