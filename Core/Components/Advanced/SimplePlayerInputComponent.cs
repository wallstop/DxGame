using System;
using System.Diagnostics;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Advanced
{
    public class SimplePlayerInputComponent : UpdateableComponent
    {
        private static readonly float ACCELERATE_AMOUNT = 0.2f;
        private static readonly float GRAVITY = 2.5f;
        private static readonly float DECAY_AMOUNT = 0.1f;
        private static readonly float JUMP_SPEED = 20.0f;
        protected PhysicsComponent physics_;

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

        public override bool Update(GameTime gameTime)
        {
            HandleInput();
            return true;
        }

        protected virtual void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            Keys[] pressedKeys = keyboardState.GetPressedKeys();
            Vector2 acceleration = physics_.Acceleration;
            Vector2 velocity = physics_.Velocity;
            if (pressedKeys.Length > 0)
            {
                foreach (Keys key in pressedKeys)
                {
                    switch (key)
                    {
                    case Keys.Left:
                        acceleration.X -= ACCELERATE_AMOUNT;
                        break;
                    case Keys.Right:
                        acceleration.X += ACCELERATE_AMOUNT;
                        break;
                    case Keys.Up:
                        if (!physics_.IsJumping)
                        {
                            velocity.Y = -JUMP_SPEED;
                            physics_.IsJumping = true;
                            acceleration.Y = GRAVITY;
                        }
                        break;
                    case Keys.Down:
                        acceleration.Y += ACCELERATE_AMOUNT;
                        break;
                    }
                }
            }

            // TODO: Better acceleration. This is hilariously bad
            if (Math.Abs(acceleration.X) - DECAY_AMOUNT > 0.0f)
            {
                acceleration.X -= DECAY_AMOUNT * MathUtils.SignOf(acceleration.X);
            }
            else
            {
                acceleration.X = 0.0f;
            }

            if (Math.Abs(acceleration.Y) - DECAY_AMOUNT > 0.0f)
            {
                acceleration.Y -= DECAY_AMOUNT * MathUtils.SignOf(acceleration.Y);
            }
            //else
            //{
            //    acceleration.Y = 0.0f;
            //}

            physics_.Acceleration = acceleration;
            physics_.Velocity = velocity;
        }
    }
}