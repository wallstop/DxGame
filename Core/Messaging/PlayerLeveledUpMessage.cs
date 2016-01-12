using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging
{
    /**
        <summary>
            Notifies listeners that a Player has reached a new experience level
        </summary>
    */

    [DataContract]
    [Serializable]
    public class PlayerLeveledUpMessage : Message
    {
        [DataMember]
        public GameObject Player { get; }

        [DataMember]
        public int NewLevel { get; }

        public PlayerLeveledUpMessage(GameObject player, int newLevel)
        {
            Validate.IsNotNullOrDefault(player, StringUtils.GetFormattedNullOrDefaultMessage(this, player));
            Validate.IsTrue(newLevel > 0,
                $"Cannot create a {typeof(PlayerLeveledUpMessage)} with a {nameof(newLevel)} of {newLevel}");
            Player = player;
            NewLevel = newLevel;
        }
    }
}