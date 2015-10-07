using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Player
{
    [Serializable]
    [DataContract]
    public class PlayerInputListener : Component
    {
        private static readonly TimeSpan DROP_THROUGH_PLATFORM_DELAY = TimeSpan.FromSeconds(1);

        [DataMember]
        private TimeSpan lastDroppedThroughPlatform_ = TimeSpan.FromSeconds(0);

        public PlayerInputListener(DxGame game) 
            : base(game)
        {
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* TODO: Change state transitions to simply be event handlers based off the events emitted from here */
            var inputModel = DxGame.Model<InputModel>();
            CheckDropThroughPlatform(inputModel, gameTime);
            CheckMovementCommands(inputModel, gameTime);
            base.Update(gameTime);
        }

        private void CheckDropThroughPlatform(InputModel inputModel, DxGameTime gameTime)
        {
            if (inputModel.Events.Any(keyEvent => keyEvent.Key == DxGame.Controls.Down && keyEvent.HeldDown) && 
                (lastDroppedThroughPlatform_ + DROP_THROUGH_PLATFORM_DELAY) < gameTime.TotalGameTime)
            {
                Parent?.BroadcastMessage(new DropThroughPlatformRequest());
                lastDroppedThroughPlatform_ = gameTime.TotalGameTime;
            }
        }

        private void CheckMovementCommands(InputModel inputModel, DxGameTime gameTime)
        {
            var inputEvents = inputModel.Events.ToList();
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Left))
            {
                Parent?.BroadcastMessage(new CommandMessage() {Commandment = Commandment.MoveLeft});
            }
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Right))
            {
                Parent?.BroadcastMessage(new CommandMessage() { Commandment = Commandment.MoveRight });
            }
            if (inputEvents.Any(inputEvent => inputEvent.Key == DxGame.Controls.Jump))
            {
                Parent?.BroadcastMessage(new CommandMessage() { Commandment = Commandment.MoveUp });
            }
        }
    }
}
