using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced.Player
{
    [DataContract]
    [Serializable]
    public class PlayerNameComponent : Component
    {
        [DataMember]
        public string Name { get; private set; }

        public PlayerNameComponent(string playerName)
        {
            Validate.IsNotNullOrDefault(playerName, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(playerName)));
            Name = playerName;
        }
    }
}
