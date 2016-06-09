using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced.Player
{
    [DataContract]
    [Serializable]
    public class PlayerNameComponent : Component
    {
        [DataMember]
        public string Name { get; private set; }

        public PlayerNameComponent(string playerName)
        {
            Validate.IsNotNullOrDefault(playerName, this.GetFormattedNullOrDefaultMessage(nameof(playerName)));
            Name = playerName;
        }
    }
}
