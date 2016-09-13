using DxCore.Core;
using DxCore.Core.Components.Basic;
using NLog;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Components.Advanced.Behaviors
{
    // TODO: 
    /** 
     * This class logic can be almost entirely static, but it will need 
     * 1) to be populated with unique attribute data 
     * 2) custom but hopefully reuseable logic for "map goal of type X to commandment chain Y" 
     * 
     * Abstract class will break the Builder pattern; maybe expand the constructor to include an 'attribute map" and a "goal resolver"? 
     */

    [Serializable]
    [DataContract]
    public class BehaviorComponent : Component
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        ////? Swanky tags required to support networked play?  Better ask Eli
        //public Team Team { get; }
        //public ToDoList ToDoList { get; private set; }

        //protected BehaviorComponent(Team team)
        //{
        //    Team = team;
        //}

        //public void AssignGoals(ToDoList toDoList)
        //{
        //    LOG.Info($"{this.GetType().Name} assigned goals: {toDoList.Goals}");
        //    // No checks here: BehaviorComponents don't know when they should get a new list of goals, they have to take our word for it
        //    ToDoList = toDoList;
        //}

        ///// <summary>
        ///// Update and inspect the currently assigned goal, generating and emitting any messages required to support it
        ///// </summary>
        ///// <param name="gameTime"></param>
        //protected override void Update(DxGameTime gameTime)
        //{
        //    Optional<Goal> maybeNextIncompleteGoal = ToDoList.NextIncompleteGoal();
        //    if (maybeNextIncompleteGoal.HasValue)
        //    {
        //        Goal nextIncompleteGoal = maybeNextIncompleteGoal.Value;
        //        try
        //        {
        //            /* This is a just-in-time approach to goal interpretation-- 
        //             * the interpreter has to maintain an internal state so it knows what Message objects to return,
        //             * as opposed to mapping [Goals] -> [Messages] all at once.  This allows GoalInterpreters to
        //             * be a lot "smarter" (if they wish), but it might be a bad idea, especially if it creates a lot 
        //             * of overhead when we try to write GoalInterpreters.
        //             * 
        //             * We could also map "goal types" : "goal interpreter factories", and create a new interpreter
        //             * whenever we have a new goal.  That's nice and clean but requires us to write a new GoalInterpreterFactory
        //             * for each new goal; 2N
        //             */
        //            GoalInterpreter<Goal> interpreter = Interpreters[nextIncompleteGoal.GetType()];
        //            //? Who should own "currently working on goal x"?  
        //            //? Who should be informed of "moving on to goal y"?
        //            // Interpreters can extrapolate by reference checks but that seems stupid
        //            // So stupid
        //            IEnumerable<Message> messages = interpreter.GenerateMessages(gameTime, this, nextIncompleteGoal);
        //            foreach (Message message in messages)
        //            {
        //                message.EmitUntyped();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // No interpreter for this goal type?  That's a paddlin'
        //            string entityName = Parent?.ComponentOfType<EntityTypeComponent>().EntityType.Name;
        //            LOG.Error($"Unable to find an interpreter for goal ${nextIncompleteGoal} of type {entityName}?");
        //            throw ex;
        //        }
        //    }
        //}

        //public static BehaviorComponentBuilder Builder()
        //{
        //    return new BehaviorComponentBuilder();
        //}

        //public class BehaviorComponentBuilder : IBuilder<BehaviorComponent>
        //{
        //    private Team team_;
        //    private Dictionary<Type, GoalInterpreter<Goal>> interpreters_ = new Dictionary<Type, GoalInterpreter<Goal>>();

        //    public BehaviorComponent Build()
        //    {
        //        Validate.IsNotNull(team_, StringUtils.GetFormattedNullOrDefaultMessage(this, team_));
        //        Validate.IsNotEmpty(interpreters_, StringUtils.GetFormattedNullOrDefaultMessage(this, interpreters_));

        //        return new BehaviorComponent(team_, interpreters_);
        //    }

        //    public BehaviorComponentBuilder WithTeam(Team team)
        //    {
        //        team_ = team;
        //        return this;
        //    }

        //    //? Is there a simpler/more robust way of doing this mapping? 
        //    public BehaviorComponentBuilder WithGoalInterpreter(Type goalType, GoalInterpreter<Goal> interpreter)
        //    {
        //        interpreters_.Add(goalType, interpreter);
        //        return this;
        //    }
        //}
    }
}