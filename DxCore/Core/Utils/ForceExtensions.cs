using DxCore.Core.Models;
using DxCore.Core.Primitives;

namespace DxCore.Core.Utils
{
    public static class ForceExtensions
    {
        public static DxVector2 FarseerScaled(this DxVector2 force)
        {
            return force / DxGame.Instance.PhysicsUpdateFrequency * WorldModel.DxToFarseerScale;
        }
    }
}
