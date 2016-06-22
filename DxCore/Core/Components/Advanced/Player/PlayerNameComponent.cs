using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

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
            Validate.Hard.IsNotNullOrDefault(playerName, this.GetFormattedNullOrDefaultMessage(nameof(playerName)));
            Name = playerName;
        }
    }
}
