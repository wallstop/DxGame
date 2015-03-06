﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    public class SimplePlayerInputComponent : Component
    {
        private const float JUMP_SPEED = 10.0f;
        private const float MOVE_SPEED = 10.0f;
        public string StateRequest { get; set; }
        protected PhysicsComponent physics_;
        protected StateComponent state_;
        protected WeaponComponent weapon_;

        private Vector2 lastAcceleration_;

        public SimplePlayerInputComponent(DxGame game)
            : base(game)
        {
            UpdatePriority = UpdatePriority.HIGH;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(physics), "SimplePlayerInput cannot be assigned a null PhysicsComponent");
            physics_ = physics;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerState(StateComponent state)
        {
            Debug.Assert(!GenericUtils.IsNullOrDefault(state), "SimplePlayerInput cannot be initialized with a null PlayerState");
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

        public override void Update(GameTime gameTime)
        {
            IEnumerable<KeyboardEvent> events = DxGame.Model<InputModel>().Events;
            HandleInput(events, gameTime);
        }

        protected virtual void HandleInput(IEnumerable<KeyboardEvent> events, GameTime gameTime)
        {
            Vector2 acceleration = physics_.Acceleration;
            Vector2 velocity = physics_.Velocity;
            string state = state_.State;

            bool isMovingLeft = false;
            bool isMovingRight = false;

            foreach (KeyboardEvent keyEvent in events)
            {
                switch (keyEvent.Key)
                {
                    case Keys.Left:
                        if (velocity.X < 0)
                        {
                            velocity.X = 1.5f * -MOVE_SPEED;
                        }
                        else
                        {
                            velocity.X = -MOVE_SPEED;
                        }
                        isMovingLeft = true;
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
                        isMovingRight = true;
                        break;
                    case Keys.Up:
                        if (state_.State != "Jumping")
                        {
                            StateRequest = "Jumping";
                            velocity.Y -= JUMP_SPEED;
                            acceleration.Y -= JUMP_SPEED;
                        }
                        break;
                    case Keys.Down:
                        break;
                    case Keys.Space:
                        weapon_.Attack(gameTime);
                        break;
                }
            }

            if (MathUtils.FuzzyCompare(lastAcceleration_.Y, 0) == 0 && MathUtils.FuzzyCompare(acceleration.Y, 0) == 0 && MathUtils.FuzzyCompare(velocity.Y, 0) == 0)
            {
                StateRequest = "None";
            }
            if ((StateRequest != "Jumping"))
            {
                if (isMovingLeft)
                {
                    StateRequest = "Walking_Left";
                }
                else if (isMovingRight)
                {
                    StateRequest = "Walking_Right";
                }
            }
            if (!(isMovingLeft || isMovingRight))
            {
                velocity.X = 0;
            }

            physics_.Acceleration = acceleration;
            physics_.Velocity = velocity;
            state_.State = state;

            lastAcceleration_ = physics_.Acceleration;
        }
    }
}