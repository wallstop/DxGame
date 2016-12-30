using System;
using System.Runtime.Serialization;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class NewPlayerNotification : Message
    {
        [DataMember]
        public Player Player { get; private set; }

        public NewPlayerNotification(Player player)
        {
            Validate.Hard.IsNotNull(player);
            Player = player;
        }
    }
}