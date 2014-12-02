using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components
{
    public class SimplePlayerInputComponent : UpdateableComponent
    {
        private static readonly float ACCELERATE_AMOUNT = 0.2f;
        private static readonly float DECAY_AMOUNT = 0.1f;
        protected PhysicsComponent physics_;

        public SimplePlayerInputComponent(PhysicsComponent physics = null, GameObject parent = null)
            : base(parent)
        {
            physics_ = physics;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Debug.Assert(physics != null, "PhysicsComponent cannot be null");
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
                        acceleration.Y -= ACCELERATE_AMOUNT;
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
                acceleration.X -= DECAY_AMOUNT * SignOf(acceleration.X);
            }
            else
            {
                acceleration.X = 0.0f;
            }

            if (Math.Abs(acceleration.Y) - DECAY_AMOUNT > 0.0f)
            {
                acceleration.Y -= DECAY_AMOUNT * SignOf(acceleration.Y);
            }
            else
            {
                acceleration.Y = 0.0f;
            }

            physics_.Acceleration = acceleration;
        }

        // TODO: Move somewhere else
        protected static int SignOf(float x)
        {
            return x > 0.0f ? 1 : -1;
        }
    }
}