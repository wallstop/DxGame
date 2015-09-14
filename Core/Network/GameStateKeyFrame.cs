﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Network
{
    /*
        Short and sweet All-Data dump of a client's gamestate
    */

    [Serializable]
    [DataContract]
    public class GameStateKeyFrame : NetworkMessage
    {
        [DataMember] public DxGameTime GameTime = new DxGameTime();

        [DataMember]
        public GameElementCollection GameElements { get; set; }

        [DataMember]
        public GameElementCollection NewGameElements { get; set; }

        [DataMember]
        public GameElementCollection RemovedGameEleemnts { get; set; }

        public static GameStateKeyFrame FromGame(DxGame game, DxGameTime gameTime)
        {
            var keyFrame = new GameStateKeyFrame
            {
                GameTime = gameTime,
                GameElements = new GameElementCollection(game.DxGameElements),
                NewGameElements = new GameElementCollection(game.NewGameElements),
                RemovedGameEleemnts = new GameElementCollection(game.RemovedGameElements)
            };
            Predicate<object> shouldSerialize = entity =>
            {
                var component = entity as Component;
                return component != null && !component.ShouldSerialize;
            };
            keyFrame.GameElements.Remove(shouldSerialize);
            keyFrame.NewGameElements.Remove(shouldSerialize);
            keyFrame.RemovedGameEleemnts.Remove(shouldSerialize);
            return keyFrame;
        }
    }
}