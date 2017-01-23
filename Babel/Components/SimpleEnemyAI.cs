using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace Babel.Components
{
    [Serializable]
    [DataContract]
    public class SimpleEnemyAI : AbstractCommandComponent
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(3);

        private static readonly TimeSpan PATHFINDING_TIMEOUT = MOVEMENT_DELAY_CHECK;
        // just cuz

        private static readonly double PERSONAL_SPACE = 50;

        [DataMember] private TimeSpan timeSinceLastMovementRequest_ = TimeSpan.Zero;

        [DataMember]
        private IPositional Positional { get; set; }

        private SimpleEnemyAI(IPositional positional)
        {
            Positional = positional;
        }

        public static SimpleEnemyAIBuilder Builder()
        {
            return new SimpleEnemyAIBuilder();
        }

        protected override void Update(DxGameTime gameTime)
        {
            timeSinceLastMovementRequest_ += gameTime.ElapsedGameTime;
            // Extract player model, players; handle limitations on AI
            PlayerService playerService = DxGame.Instance.Service<PlayerService>();
            DxCore.Core.Player player = playerService.Players.First();

            var closeEnough = (Math.Abs(player.Position.Position.X - Positional.WorldCoordinates.X) < PERSONAL_SPACE) &&
                              (Math.Abs(player.Position.Position.Y - Positional.WorldCoordinates.Y) < PERSONAL_SPACE);
            if(closeEnough)
            {
                return;
            }

            if(timeSinceLastMovementRequest_ <= MOVEMENT_DELAY_CHECK)
            {
                return;
            }

            PathfindingRequest pathfindingRequest = new PathfindingRequest(Positional.WorldCoordinates,
                player.Position.Center, Parent?.Id);
            pathfindingRequest.Emit();
            timeSinceLastMovementRequest_ = TimeSpan.Zero;
        }

        /// <summary>
        ///     A simple IBuilder for simple AI.
        ///     Requires a spatial component.
        /// </summary>
        public class SimpleEnemyAIBuilder : IBuilder<SimpleEnemyAI>
        {
            private IPositional positional_;

            public SimpleEnemyAI Build()
            {
                Validate.Hard.IsNotNull(positional_, "AI requires a Positional component to make decisions.");
                // Construct and return the desired object
                return new SimpleEnemyAI(positional_);
            }

            public SimpleEnemyAIBuilder WithPositional(IPositional positional)
            {
                positional_ = positional;
                return this;
            }
        }
    }
}