using DxCore.Core.Components.Advanced.Behaviors;
using WallNetCore.Validate;

namespace DxCore.Core.Utils
{
    public static class BehaviorUtils
    {
        public static AffinityComponent AffinityComponent(this BehaviorComponent behaver)
        {
            AffinityComponent affinityComponent = behaver.Parent?.ComponentOfType<AffinityComponent>();
            Validate.Hard.IsNotNull(affinityComponent, $"Could not acquire an affinity component for {behaver}!");
            return affinityComponent;
        }
    }
}