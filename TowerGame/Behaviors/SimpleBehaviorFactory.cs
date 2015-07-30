using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.State;
using DXGame.Main;

namespace DXGame.TowerGame.Behaviors
{
    public class SimpleBehaviorFactory
    {
        public static StateMachine SimpleActivePlayerProximityTriggerFollow(DxGame game, PositionalComponent position)
        {
            /* 
                TODO: How do we build generic Behaviors? IE, have a behavior that is 
                desynced from it's state's Actions and Presentations? Should we allow 
                States to be mutable and simply assign them later? 
            */
            return null;
            //int maxFollowDistance = 150;

            //var stateBuilder = State.Builder().WithName("Idle").WithTransition()
            //State idleState = new State("Idle");
            //State followState = new State("Following Player");
            //Transition idleToFollowTransition = new Transition(() =>
            //{
            //    var playerPosition = game.Model<PlayerModel>().ActivePlayer.Position;
            //    var distance = Vector2.Distance(playerPosition.Position.ToVector2(), position.Position.ToVector2());
            //    return distance < maxFollowDistance;
            //}, followState);

            //Transition followToIdleTransition = new Transition(() =>
            //{
            //    var playerPosition = game.Model<PlayerModel>().ActivePlayer.Position;
            //    var distance = Vector2.Distance(playerPosition.Position.ToVector2(), position.Position.ToVector2());
            //    return distance >= maxFollowDistance;
            //}, idleState);

            //idleState.WithTransition(idleToFollowTransition);
            //followState.WithTransition(followToIdleTransition);

            //StateMachine activePlayerFollowBehavior = new StateMachine(idleState);
            //return activePlayerFollowBehavior;
        }
    }
}