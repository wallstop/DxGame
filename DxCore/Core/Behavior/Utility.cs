using DxCore.Core.Behavior.Goals;
using DxCore.Core.Frames;

namespace DxCore.Core.Behavior
{
    /**
        This UtilityFunction should generally be a member-of or have-access-to a 
        particular entity's internal state.

        <summary>
            Given both the "type" of goal and an "end goal" of gamestate as a result,
            determines the utility of the action as it pertains to a specific entity
        </summary>
    */

    public delegate Score UtilityFunction(ActionType goalType, Frame result);
}
