using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Command;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.TowerGame.Components
{
    [Serializable]
    [DataContract]
    public class SimpleEnemyAI : AbstractCommandComponent
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(3);

        private static readonly TimeSpan PATHFINDING_TIMEOUT = MOVEMENT_DELAY_CHECK;
            // just cuz

        private static readonly double PERSONAL_SPACE = 50;

        [DataMember] private readonly SpatialComponent spatialComponent_;

        private TimeSpan timeSinceLastMovementRequest_ = TimeSpan.Zero;

        /// <summary>
        ///     Initialize a simple AI that follows the player.
        /// </summary>
        /// <param name="spatialComponent">  This AI's object's spatial component.</param>
        private SimpleEnemyAI(SpatialComponent spatialComponent)
        {
            spatialComponent_ = spatialComponent;
        }

        protected override void Update(DxGameTime gameTime)
        {
            timeSinceLastMovementRequest_ += gameTime.ElapsedGameTime;
            // Extract player model, players; handle limitations on AI
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
            Validate.AreEqual(1, playerModel.Players.Count,
                $"{GetType()} cannot fathom {playerModel.Players.Count} players.");
            Core.Player player = playerModel.Players.First();

            var closeEnough = Math.Abs(player.Position.Position.X - spatialComponent_.Position.X) < PERSONAL_SPACE && Math.Abs(player.Position.Position.Y - spatialComponent_.Position.Y) < PERSONAL_SPACE;
            if (closeEnough)
            {
                return;
            }

            if (timeSinceLastMovementRequest_ <= MOVEMENT_DELAY_CHECK)
            {
                return;
            }

            var pathfindingRequest = new PathFindingRequest
            {
                Location = new DxVector2(player.Position.Position.X, player.Position.Position.Y + player.Position.Height - 0.001),
                Timeout = PATHFINDING_TIMEOUT
            };
            Parent?.BroadcastMessage(pathfindingRequest);
            timeSinceLastMovementRequest_ = TimeSpan.Zero;
        }

        public static SimpleEnemyAIBuilder Builder()
        {
            return new SimpleEnemyAIBuilder();
        }

        /// <summary>
        ///     A simple IBuilder for simple AI.
        ///     Requires a spatial component.
        /// </summary>
        public class SimpleEnemyAIBuilder : IBuilder<SimpleEnemyAI>
        {
            private SpatialComponent spatialComponent_;

            public SimpleEnemyAI Build()
            {
                Validate.IsNotNull(spatialComponent_, "AI requires a spatial component to make decisions.");
                // Construct and return the desired object
                return new SimpleEnemyAI(spatialComponent_);
            }

            public SimpleEnemyAIBuilder WithSpatialComponent(SpatialComponent spatialComponent)
            {
                spatialComponent_ = spatialComponent;
                return this;
            }
        }
    }
}