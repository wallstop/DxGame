using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
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
                GameElements = game.DxGameElements,
                NewGameElements = game.NewGameElements,
                RemovedGameEleemnts = game.RemovedGameElements
            };
            return keyFrame;
        }
    }
}