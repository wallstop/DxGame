using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Wrappers;

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
        [DataMember] public List<Component> Components = new List<Component>();
    }
}