using System.Collections.Generic;
using DXGame.Core.Components.Advanced.Physics;
using DXGame.Core.Components.Advanced.Properties;
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
        protected EntityPropertiesComponent EntityProperties;
        // TODO: Move these out
        protected PhysicsComponent physics_;
        protected StateComponent state_;
        protected WeaponComponent weapon_;

        public SimplePlayerInputComponent(DxGame game)
            : base(game)
        {
            UpdatePriority = UpdatePriority.HIGH;
        }

        public SimplePlayerInputComponent WithPhysics(PhysicsComponent physics)
        {
            Validate.IsNotNullOrDefault(physics, $"Cannot initialize {GetType()} with a null/default PhysicsComponent");
            physics_ = physics;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerState(StateComponent state)
        {
            Validate.IsNotNullOrDefault(state, $"Cannot intialize {GetType()} with a null/default PlayerState");
            state_ = state;
            return this;
        }

        public SimplePlayerInputComponent WithWeapon(WeaponComponent weapon)
        {
            Validate.IsNotNullOrDefault(weapon, $"Cannot initialize {GetType()} with a null/default weapon");
            weapon_ = weapon;
            return this;
        }

        public SimplePlayerInputComponent WithPlayerProperties(EntityPropertiesComponent properties)
        {
            Validate.IsNotNullOrDefault(properties, $"Cannot initialize {GetType()} with null/default EntityProperties");
            EntityProperties = properties;
            return this;
        }

        protected override void Update(DxGameTime gameTime)
        {
            IEnumerable<KeyboardEvent> events = DxGame.Model<InputModel>().Events;
            HandleInput(events, gameTime);
        }

        // TODO: Un-hardcode these values
        // TODO: Move these to some functors or some shit
        private void MoveLeft(StateChangeRequestMessage request)
        {
            if (physics_.Velocity.X < 0)
            {
                // TODO: Un-hard code this
                physics_.Velocity = new DxVector2(1.5f * -EntityProperties.MoveSpeed.CurrentValue,
                    physics_.Velocity.Y);
            }
            else
            {
                physics_.Velocity = new DxVector2(-EntityProperties.MoveSpeed.CurrentValue,
                    physics_.Velocity.Y);
            }
            if (request.State != "Jumping")
            {
                request.State = "Walking_Left";
            }
        }

        private void MoveRight(StateChangeRequestMessage request)
        {
            if (physics_.Velocity.X < 0)
            {
                physics_.Velocity = new DxVector2(1.5f * EntityProperties.MoveSpeed.CurrentValue,
                    physics_.Velocity.Y);
            }
            else
            {
                physics_.Velocity = new DxVector2(EntityProperties.MoveSpeed.CurrentValue,
                    physics_.Velocity.Y);
            }
            if (request.State != "Jumping")
            {
                request.State = "Walking_Right";
            }
        }

        private void Jump(StateChangeRequestMessage request)
        {
            if (state_.State != "Jumping")
            {
                request.State = "Jumping";
                // In case we're falling, set our y-wards velocity to nothing so we get the full jump
                physics_.Velocity = new DxVector2(physics_.Velocity.X, 0);
                physics_.Acceleration = new DxVector2(physics_.Acceleration.X,
                    -EntityProperties.JumpSpeed.CurrentValue);
            }
        }

        private void CheckForLackOfMovement(StateChangeRequestMessage request)
        {
            if (request.State == "None")
            {
                // If we aren't moving left or right (or requested a jump this frame), stop moving horizontally
                physics_.Velocity = new DxVector2(0, physics_.Velocity.Y);
            }
        }

        protected virtual void HandleInput(IEnumerable<KeyboardEvent> events, DxGameTime gameTime)
        {
            StateChangeRequestMessage request = new StateChangeRequestMessage {State = "None"};

            foreach (KeyboardEvent keyEvent in events)
            {
                switch (keyEvent.Key)
                {
                    case Keys.Left:
                        MoveLeft(request);
                        break;
                    case Keys.Right:
                        MoveRight(request);
                        break;
                    case Keys.Up:
                        Jump(request);
                        break;
                    case Keys.Down:
                        break;
                    case Keys.Space:
                        weapon_.Attack(gameTime);
                        break;
                }
            }

            CheckForLackOfMovement(request);
            Parent.BroadcastMessage(request);
        }
    }
}