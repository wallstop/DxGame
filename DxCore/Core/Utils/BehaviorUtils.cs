using DXGame.Core.Components.Advanced.Behaviors;

namespace DxCore.Core.Utils
{
    public static class BehaviorUtils
    {
        public static AffinityComponent AffinityComponent(this BehaviorComponent behaver)
        {
            AffinityComponent affinityComponent = behaver.Parent?.ComponentOfType<AffinityComponent>();
            Validate.Validate.Hard.IsNotNull(affinityComponent, $"Could not acquire an affinity component for {behaver}!");
            return affinityComponent;
        }
    }
}
