using System;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    public class SimplePlayerInputComponent : UpdateableComponent
    {
        //ivate static readonly float DECAY_AMOUNT = 0.1f;
        private static readonly float JUMP_SPEED = 10.0f;
        private static readonly float MOVE_SPEED = 10.0f;
        protected PhysicsComponent physics_;
        protected PlayerStateComponent state_;

        private Vector2 lastAcceleration = new Vector2();

        public SimplePlayerInputComponent(PhysicsComponent physics = null, GameObject parent = null)
            : base(parent)
        {
            physics_ = physics;
            priority_ = UpdatePriority.HIGH;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Debug.Assert(physics != null, "SimplePlayerInput cannot be assigned a null PhysicsComponent");
            physics_ = physics;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerState(PlayerStateComponent state)
        {
            Debug.Assert(state != null, "SimplePlayerInput cannot be initialized with a null PlayerState");
            state_ = state;
            return this;
        }

        public override bool Update(GameTime gameTime)
        {
            HandleInput();
            return true;
        }

        protected virtual void HandleInput()
        {
            Vector2 acceleration = physics_.Acceleration;
            Vector2 velocity = physics_.Velocity;
            PlayerState state = state_.State;

            bool isMoving = false;

            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            if (pressedKeys.Length > 0)
            {
                foreach (Keys key in pressedKeys)
                {
                    switch (key)
                    {
                    case Keys.Left:
                        velocity.X = -MOVE_SPEED;
                        isMoving = true;
                        break;
                    case Keys.Right:
                        velocity.X = MOVE_SPEED;
                        isMoving = true;
                        break;
                    case Keys.Up:
                        // TODO: Remove shit code, replace with proper collision.
                        switch (state_.State)
                        {
                        case PlayerState.Walking:
                        case PlayerState.None:
                            state = PlayerState.Jumping;
                            velocity.Y -= JUMP_SPEED;
                            acceleration.Y -= JUMP_SPEED;
                            break;
                        case PlayerState.Jumping:
                            if (lastAcceleration.Y == 0  && acceleration.Y == 0 && velocity.Y == 0)
                            {
                                state = PlayerState.None;
                            }
                            break;

                        default:
                            break;
                        }
                        break;
                    case Keys.Down:
                        break;
                    }
                }
            }
            //Really just want to know if they never pressed left or right so
            //horizontal movement can be stopped even while jumping. 
            if(!isMoving)
            {
                velocity.X = 0;
            }

            physics_.Acceleration = acceleration;
            physics_.Velocity = velocity;
            state_.State = state;

            lastAcceleration = physics_.Acceleration;
        }
    }
}