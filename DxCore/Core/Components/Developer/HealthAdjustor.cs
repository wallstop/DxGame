using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Primitives;
using DxCore.Core.Properties;
using DxCore.Core.Services;
using Microsoft.Xna.Framework.Input;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary>
            Screws around with the active player's health
        </summary>
    */

    [DataContract]
    [Serializable]
    public class HealthAdjustor : Component
    {
        private static readonly Keys HEALTH_DOWN = Keys.J;
        private static readonly Keys HEALTH_UP = Keys.K;

        private static readonly int HEALTH_SCALE = 2;

        protected override void Update(DxGameTime gameTime)
        {
            InputService input = DxGame.Instance.Service<InputService>();
            PlayerService playerService = DxGame.Instance.Service<PlayerService>();

            if(ReferenceEquals(input, null) || ReferenceEquals(playerService, null))
            {
                return;
            }

            Player activePlayer = playerService.ActivePlayer;
            if(ReferenceEquals(activePlayer, null))
            {
                return;
            }

            EntityProperties entityProperties =
                activePlayer.Object.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            foreach(KeyboardEvent keyboardEvent in input.InputHandler.FinishedKeyboardEvents)
            {
                if(keyboardEvent.Source == HEALTH_DOWN)
                {
                    entityProperties.Health.BaseValue -= HEALTH_SCALE;
                }
                else if(keyboardEvent.Source == HEALTH_UP)
                {
                    entityProperties.Health.BaseValue += HEALTH_SCALE;
                }
            }
        }
    }
}