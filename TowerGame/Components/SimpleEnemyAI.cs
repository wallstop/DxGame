using System;
using DXGame.Core.Models;
using DXGame.Core.Components.Basic;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;
using System.Linq;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Components
{
    public class SimpleEnemyAI : Component
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(2);
        private readonly SpatialComponent spatialComponent_;
        private TimeSpan lastChangedMovement_ = TimeSpan.FromSeconds(0);

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

            // TODO(mat): Re-implement perhaps once a player model is working
            /*
            PlayerModel playerModel = DxGame.Model<PlayerModel>();
            if (playerModel.Players.Count > 1) {
                throw new InvalidOperationException("SimpleEnemyAI cannot fathom multiple players.");
            } else if (playerModel.Players.Count < 1)
            {
                throw new InvalidOperationException("SimpleEnemyAI cannot cope with loneliness.");
            }
            Core.Player player = playerModel.Players.First();
            */
            Core.Player player = DxGame.Model<PlayerModel>().ActivePlayer;

            // Chase the player along the X-axis, sending a movement request if the player moves away
            if (player.Position.Position.X < spatialComponent_.Position.X)
            {
                Parent.BroadcastMessage(new MovementRequest { Direction = Direction.West });
            } else if (player.Position.Position.X > spatialComponent_.Position.X)
            {
                Parent.BroadcastMessage(new MovementRequest { Direction = Direction.East });
            }
            /*
            if (lastChangedMovement_ + MOVEMENT_DELAY_CHECK < gameTime.TotalGameTime)
            {
                Random rGen = new Random();
                direction_ = rGen.Next() % 2 == 0 ? Direction.East : Direction.West;
                lastChangedMovement_ = gameTime.TotalGameTime;
            }

            var movementRequest = new MovementRequest {Direction = direction_};
            Parent.BroadcastMessage(movementRequest);
            */
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
                Validate.IsNotNull<DxGame>(game_,
                    "AI requires a game to function.");
                Validate.IsNotNull<SpatialComponent>(spatialComponent_, 
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
