﻿using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced.Player
{
    // Should not be serialized
    public class SimplePlayerInputComponent : Component
    {
        private const float JUMP_SPEED = 10.0f;
        private const float MOVE_SPEED = 10.0f;
        private DxVector2 lastAcceleration_;
        protected PhysicsComponent physics_;
        protected StateComponent state_;
        protected WeaponComponent weapon_;

        public SimplePlayerInputComponent(DxGame game)
            : base(game) { UpdatePriority = UpdatePriority.HIGH; }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(physics),
                "SimplePlayerInput cannot be assigned a null PhysicsComponent");
            physics_ = physics;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerState(StateComponent state)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(state),
                "SimplePlayerInput cannot be initialized with a null PlayerState");
            state_ = state;
            return this;
        }

        public SimplePlayerInputComponent WithWeapon(WeaponComponent weapon)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(weapon),
                "SimplePlayerInput cannot be initialized with a null weapon");
            weapon_ = weapon;
            return this;
        }

        protected override void Update(DxGameTime gameTime)
        {
            IEnumerable<KeyboardEvent> events = DxGame.Model<InputModel>().Events;
            HandleInput(events, gameTime);
        }

        protected virtual void HandleInput(IEnumerable<KeyboardEvent> events, DxGameTime gameTime)
        {
            var acceleration = physics_.Acceleration;
            var velocity = physics_.Velocity;
            string state = state_.State;

            StateChangeRequestMessage request = new StateChangeRequestMessage {State = "None"};

            foreach (KeyboardEvent keyEvent in events)
            {
                switch (keyEvent.Key)
                {
                // TODO: Un-hardcode these values
                // TODO: Move these to some functors or some shit
                case Keys.Left:
                    if (velocity.X < 0)
                    {
                        velocity.X = 1.5f * -MOVE_SPEED;
                    }
                    else
                    {
                        velocity.X = -MOVE_SPEED;
                    }
                    if (request.State != "Jumping")
                    {
                        request.State = "Walking_Left";
                    }
                    break;
                case Keys.Right:
                    if (velocity.X > 0)
                    {
                        velocity.X = 1.5f * MOVE_SPEED;
                    }
                    else
                    {
                        velocity.X = MOVE_SPEED;
                    }
                    if (request.State != "Jumping")
                    {
                        request.State = "Walking_Right";
                    }
                    break;
                case Keys.Up:
                    if (state_.State != "Jumping")
                    {
                        request.State = "Jumping";
                        velocity.Y = -JUMP_SPEED;
                        acceleration.Y = -JUMP_SPEED;
                    }
                    break;
                case Keys.Down:
                    break;
                case Keys.Space:
                    weapon_.Attack(gameTime);
                    break;
                }
            }

            if (state != "Jumping" && request.State == "None")
            {
                velocity.X = 0;
            }

            physics_.Acceleration = acceleration;
            physics_.Velocity = velocity;
            state_.State = state;

            lastAcceleration_ = physics_.Acceleration;

            Parent.BroadcastMessage(request);
        }
    }
}