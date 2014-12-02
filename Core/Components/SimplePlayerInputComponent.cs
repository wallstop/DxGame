using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components
{
    public class SimplePlayerInputComponent : UpdateableComponent
    {
        private static readonly float ACCELERATE_AMOUNT = 0.2f;
        protected PhysicsComponent physics_;

        public SimplePlayerInputComponent(PhysicsComponent physics = null, GameObject parent = null)
            : base(parent)
        {
            physics_ = physics;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            // TODO: null validation
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
            else
            {
                if (Math.Abs(acceleration.X) - ACCELERATE_AMOUNT > 0.0f)
                {
                    acceleration.X -= ACCELERATE_AMOUNT * signOf(acceleration.X);
                }
                else
                {
                    acceleration.X = 0.0f;
                }

                if (Math.Abs(acceleration.Y) - ACCELERATE_AMOUNT > 0.0f)
                {
                    acceleration.Y -= ACCELERATE_AMOUNT * signOf(acceleration.Y);
                }
                else
                {
                    acceleration.Y = 0.0f;
                }
            }

            physics_.Acceleration = acceleration;
        }

        protected static int signOf(float x)
        {
            return x > 0.0f ? 1 : -1;
        }
    }
}