using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Network
{
    /*
        Short and sweet All-Data dump of a client's gamestate
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class GameStateKeyFrame : NetworkMessage
    {
        [ProtoMember(1)] [DataMember] public DxGameTime GameTime = new DxGameTime();

        [ProtoMember(2)]
        [DataMember]
        public GameElementCollection GameElements { get; set; }

        [ProtoMember(3)]
        [DataMember]
        public GameElementCollection NewGameElements { get; set; }

        [ProtoMember(4)]
        [DataMember]
        public GameElementCollection RemovedGameEleemnts { get; set; }

        [ProtoMember(5)]
        [DataMember]
        public List<Model> Models { get; set; }

        public GameStateKeyFrame()
        {
            MessageType = MessageType.SERVER_DATA_KEYFRAME;
        }

        public static GameStateKeyFrame FromGame(DxGameTime gameTime)
        {
            var game = DxGame.Instance;
            var keyFrame = new GameStateKeyFrame
            {
                GameTime = gameTime,
                GameElements = new GameElementCollection(game.DxGameElements),
                NewGameElements = new GameElementCollection(game.NewGameElements),
                RemovedGameEleemnts = new GameElementCollection(game.RemovedGameElements),
                Models = new List<Model>(game.Models)
            };
            Predicate<object> shouldSerialize = entity =>
            {
                var component = entity as Component;
                return component != null && !component.ShouldSerialize;
            };
            keyFrame.GameElements.Remove(shouldSerialize);
            keyFrame.NewGameElements.Remove(shouldSerialize);
            keyFrame.RemovedGameEleemnts.Remove(shouldSerialize);
            keyFrame.Models.RemoveAll(shouldSerialize);
            return keyFrame;
        }
    }
}