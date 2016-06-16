using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Properties;
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
            InputModel input = DxGame.Instance.Model<InputModel>();
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();

            if(ReferenceEquals(input, null) || ReferenceEquals(playerModel, null))
            {
                return;
            }

            Player activePlayer = playerModel.ActivePlayer;
            if(ReferenceEquals(activePlayer, null))
            {
                return;
            }

            EntityProperties entityProperties =
                activePlayer.Object.ComponentOfType<EntityPropertiesComponent>().EntityProperties;
            foreach(KeyboardEvent keyboardEvent in input.FinishedEvents)
            {
                if(keyboardEvent.Key == HEALTH_DOWN)
                {
                    entityProperties.Health.BaseValue -= HEALTH_SCALE;
                }
                else if(keyboardEvent.Key == HEALTH_UP)
                {
                    entityProperties.Health.BaseValue += HEALTH_SCALE;
                }
            }
        }
    }
}