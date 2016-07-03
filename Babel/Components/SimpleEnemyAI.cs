﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore;
using DxCore.Core.Components.Advanced.Command;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace Babel.Components
{
    [Serializable]
    [DataContract]
    public class SimpleEnemyAI : AbstractCommandComponent
    {
        private static readonly TimeSpan MOVEMENT_DELAY_CHECK = TimeSpan.FromSeconds(2);

        private static readonly TimeSpan PATHFINDING_TIMEOUT = MOVEMENT_DELAY_CHECK;
            // just cuz

        private static readonly double PERSONAL_SPACE = 50;

        [DataMember]
        private PhysicsComponent Physics { get; set; }

        private TimeSpan timeSinceLastMovementRequest_ = TimeSpan.Zero;

        /// <summary>
        ///     Initialize a simple AI that follows the player.
        /// </summary>
        /// <param name="spatialComponent">  This AI's object's spatial component.</param>
        private SimpleEnemyAI(PhysicsComponent physics)
        {
            Physics = physics;
        }

        protected override void Update(DxGameTime gameTime)
        {
            timeSinceLastMovementRequest_ += gameTime.ElapsedGameTime;
            // Extract player model, players; handle limitations on AI
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
            DxCore.Core.Player player = playerModel.Players.First();

            var closeEnough = Math.Abs(player.Position.Position.X - Physics.Position.X) < PERSONAL_SPACE && Math.Abs(player.Position.Position.Y - Physics.Position.Y) < PERSONAL_SPACE;
            if (closeEnough)
            {
                return;
            }

            if (timeSinceLastMovementRequest_ <= MOVEMENT_DELAY_CHECK)
            {
                return;
            }

            PathFindingRequest pathfindingRequest = new PathFindingRequest
            {
                Location = new DxVector2(player.Position.Position.X, player.Position.Position.Y + player.Position.Height - 0.001),
                Timeout = PATHFINDING_TIMEOUT,
                Target = Parent?.Id
            };
            pathfindingRequest.Emit();
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
                Validate.Hard.IsNotNull(spatialComponent_, "AI requires a spatial component to make decisions.");
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