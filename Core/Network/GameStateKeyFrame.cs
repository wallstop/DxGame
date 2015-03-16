using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    /*
        Short and sweet All-Data dump of a client's gamestate
    */
    [Serializable]
    [DataContract]
    public class GameStateKeyFrame : NetworkMessage
    {
        [DataMember]
        public GameTime GameTime = new GameTime();
        [DataMember]
        public List<IGameComponent> Components = new List<IGameComponent>();
    }
}