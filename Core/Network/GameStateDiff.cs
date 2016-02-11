using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    /*
        Short and sweet list of things that are different from one timestamp to the next
    */

    [Serializable]
    [DataContract]
    public class GameStateDiff : NetworkMessage
    {
        public GameStateDiff()
        {
            MessageType = MessageType.ServerDataDiff;
        }

        //[DataMember] public GameTime GameTime = new GameTime();
        [DataMember] public List<IGameComponent> Added = new List<IGameComponent>();
        [DataMember] public List<IGameComponent> Updated = new List<IGameComponent>();
        [DataMember] public List<IGameComponent> Removed = new List<IGameComponent>();
    }
}