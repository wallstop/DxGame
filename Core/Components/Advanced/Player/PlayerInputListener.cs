using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Player
{
    public class PlayerInputListener : Component
    {
        private static readonly TimeSpan DROP_THROUGH_PLATFORM_DELAY = TimeSpan.FromSeconds(1);

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
    }
}
