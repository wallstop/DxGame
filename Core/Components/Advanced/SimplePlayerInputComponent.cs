using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    public class SimplePlayerInputComponent : Component
    {
        //ivate static readonly float DECAY_AMOUNT = 0.1f;
        private static readonly float JUMP_SPEED = 10.0f;
        private static readonly float MOVE_SPEED = 10.0f;
        protected PhysicsComponent physics_;
        protected StateComponent state_;

        private Vector2 lastAcceleration_;

        public SimplePlayerInputComponent(DxGame game)
            : base(game)
        {
            UpdatePriority = UpdatePriority.HIGH;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Debug.Assert(physics != null, "SimplePlayerInput cannot be assigned a null PhysicsComponent");
            physics_ = physics;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerState(StateComponent state)
        {
            Debug.Assert(state != null, "SimplePlayerInput cannot be initialized with a null PlayerState");
            state_ = state;
            return this;
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput();
        }

        protected virtual void HandleInput()
        {
            Vector2 acceleration = physics_.Acceleration;
            Vector2 velocity = physics_.Velocity;
            string state = state_.State;

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
                        if (velocity.X < 0)
                        {
                            velocity.X = 1.5f * -MOVE_SPEED;
                        }
                        else
                        {
                            velocity.X = -MOVE_SPEED;
                        }
                        isMoving = true;
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
                        isMoving = true;
                        break;
                    case Keys.Up:
                        // TODO: Remove shit code, replace with proper collision.
                        switch (state_.State)
                        {
                        case "Walking":
                        case "None":
                            if (state_.States.Contains("Jumping"))
                            {
                                state = "Jumping";
                                velocity.Y -= JUMP_SPEED;
                                acceleration.Y -= JUMP_SPEED;
                            }
                            break;
                        //case "Jumping":
                        //    if (lastAcceleration_.Y == 0 && acceleration.Y == 0 && velocity.Y == 0)
                        //    {
                        //        state = "None";
                        //    }
                        //    break;

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
            if (lastAcceleration_.Y == 0 && acceleration.Y == 0 && velocity.Y == 0)
            {
                state = "None";
            }
            if ((state != "Jumping") && isMoving)
            {
                state = "Walking";
            }
            if (!isMoving)
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