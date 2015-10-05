using System;
using DXGame.Core.Frames;

namespace DXGame.Core.Behavior.Goals
{
    /**
        All Abilities or Actions should end up boiling down to one of the categories
        enumerated below. The strength of enumerating these categories is that we can
        convey information about what an ability does to the behavior layer. This allows
        us to "pick" what ability to use, if any, for a behavior in a somewhat intelligent
        fashion. Without this categorization, we would have to essentially "try every ability"
        to see what effects they had, and build in intelligent ways to determine the effects!
        That's really hard.
    */

    public enum ActionType
    {
        Damage, /* An action that damages an entity */
        Movement, /* One that moves an entity */
        Help /* And one that provides aid (buffs?) to an entity */
    }


    /**
        <summary>
            Represents a discrete unit of work that can be accomplished.
        </summary>
    */
    public interface IGoal
    {
        /**
            <summary>
                What category of goal this falls into. Damage to (something)? Moving (somewhere)?
            </summary>
        */
        ActionType ActionType { get; }

        /**
            <summary>
                How long a goal should be executed before the goal executor checks back in with global coordinator
            </summary>
        */
        TimeSpan Timeout { get; }

        /**
            <summary>
                State of the world when this goal was created (reflects a base line for what we are attempting to achieve)
            </summary>
        */
        Frame Reference { get; }

        /**
            <summary>
                The resultant gamestate that will occur if the provided entity successfully executes the goal.

                Note: This is meant as a short-circuit to full gamestate playthrough & evaluation. For example,
                if the goal is "move here", then the resultant state is "the entity but with it's position over here".
            </summary>
        */
        Frame Result(GameObject entity);
        /**
            
            <summary>
                Given a frame (assumed current), determines whether or not the goal has been completed
            </summary>
        */
        bool IsComplete(Frame reference);
        /**
            <summary>
                Given a frame (assumed "potential future state that an entity's action has caused the world to be"), 
                determines the Score that that action's result represents. This should be useful for things like
                pathfinding, as-much-damage-as-possible goals, and the like.
            </summary>
        */
        Score Utility(Frame frame);
    }
}