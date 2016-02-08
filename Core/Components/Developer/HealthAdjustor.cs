﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using DXGame.Main;
using Microsoft.Xna.Framework.Input;

namespace DXGame.Core.Components.Developer
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
            Player activePlayer = playerModel.ActivePlayer;
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