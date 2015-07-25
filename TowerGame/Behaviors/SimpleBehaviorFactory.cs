using DXGame.Core.Behavior;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Models;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.TowerGame.Behaviors
{
    public class SimpleBehaviorFactory
    {
        public static Behavior SimpleActivePlayerProximityTriggerFollow(DxGame game, PositionalComponent position)
        {
            int maxFollowDistance = 150;
            State idleState = new State("Idle");
            State followState = new State("Following Player");
            Transition idleToFollowTransition = new Transition(() =>
            {
                var playerPosition = game.Model<PlayerModel>().ActivePlayer.Position;
                var distance = Vector2.Distance(playerPosition.Position.ToVector2(), position.Position.ToVector2());
                return distance < maxFollowDistance;
            }, followState);

            Transition followToIdleTransition = new Transition(() =>
            {
                var playerPosition = game.Model<PlayerModel>().ActivePlayer.Position;
                var distance = Vector2.Distance(playerPosition.Position.ToVector2(), position.Position.ToVector2());
                return distance >= maxFollowDistance;
            }, idleState);

            idleState.WithTransition(idleToFollowTransition);
            followState.WithTransition(followToIdleTransition);

            Behavior activePlayerFollowBehavior = new Behavior(idleState);
            return activePlayerFollowBehavior;
        }
    }
}