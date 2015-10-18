using System;
using DXGame.Core.Models;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Components
{
    [Serializable]
    [DataContract]
    public class SimpleEnemyAI : AbstractCommandComponent
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(1);
        private static readonly double PERSONAL_SPACE_ = 50;
        [DataMember]
        private TimeSpan lastChangedMovement_ = TimeSpan.FromSeconds(0);
        [DataMember]
        private readonly SpatialComponent spatialComponent_;
        [DataMember]
        private TimeSpan lastNearPlayer_ = TimeSpan.FromSeconds(0);

        /// <summary>
        /// Initialize a simple AI that follows the player.
        /// </summary>
        /// <param name="game">  Game instance.</param>
        /// <param name="spatialComponent">  This AI's object's spatial component.</param>
        private SimpleEnemyAI(DxGame game, SpatialComponent spatialComponent) 
            : base(game)
        {
            spatialComponent_ = spatialComponent;
        }

        protected override void Update(DxGameTime gameTime)
        {
            // Extract player model, players; handle limitations on AI
            PlayerModel playerModel = DxGame.Model<PlayerModel>();
            if (playerModel.Players.Count > 1) {
                throw new InvalidOperationException("SimpleEnemyAI cannot fathom multiple players.");
            } else if (playerModel.Players.Count < 1)
            {
                throw new InvalidOperationException("SimpleEnemyAI cannot cope with loneliness.");
            }
            Core.Player player = playerModel.Players.First();

            // Keep track of when this unit was last near the player
            if (Math.Abs(player.Position.Position.X - spatialComponent_.Position.X) < PERSONAL_SPACE_)
            {
                lastNearPlayer_ = gameTime.TotalGameTime;
            }

            // Chase the player along the X-axis, sending a movement request if the player moves away
            if (gameTime.TotalGameTime.Subtract(lastNearPlayer_) > MOVEMENT_DELAY_CHECK)
            {
                Direction d = (player.Position.Position.X > spatialComponent_.Position.X) 
                    ? Direction.East : Direction.West;
                Parent.BroadcastMessage(new CommandMessage {Commandment = CommandMessage.CommandmentForDirection(d)});
            }

            base.Update(gameTime);
        }

        public static SimpleEnemyAIBuilder Builder()
        {
            return new SimpleEnemyAIBuilder();
        }

        /// <summary>
        /// A simple IBuilder for simple AI.  
        /// 
        /// Requires a spatial component.
        /// </summary>
        public class SimpleEnemyAIBuilder : IBuilder<SimpleEnemyAI>
        {
            private DxGame game_;
            private SpatialComponent spatialComponent_;

            public SimpleEnemyAI Build()
            {
                // Validate contents of this build
                Validate.IsNotNull(game_,
                    "AI requires a game to function.");
                Validate.IsNotNull(spatialComponent_, 
                    "AI requires a spatial component to make decisions.");
                // Construct and return the desired object
                return new SimpleEnemyAI(game_, spatialComponent_);
            }

            public SimpleEnemyAIBuilder WithDxGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public SimpleEnemyAIBuilder WithSpatialComponent(SpatialComponent spatialComponent)
            {
                spatialComponent_ = spatialComponent;
                return this;
            }
        }
    }
}
