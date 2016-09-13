using DXGame.Core.Components.Advanced.Behaviors;

namespace DxCore.Core.Utils
{
    public class BehaviorUtils
    {
        public static AffinityComponent GetAffinityComponent(BehaviorComponent behaver)
        {
            AffinityComponent affinityComponent = behaver.Parent?.ComponentOfType<AffinityComponent>();
            Validate.Validate.Hard.IsNotNull(affinityComponent, $"Could not acquire an affinity component for {behaver}!");
            return affinityComponent;
        }
    }
}
