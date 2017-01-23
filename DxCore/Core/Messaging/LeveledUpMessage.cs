using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    /**
        <summary>
            Notifies listeners that an entity has reached a new experience level
        </summary>
    */

    [DataContract]
    [Serializable]
    public class LeveledUpMessage : Message
    {
        [DataMember]
        public GameObject Entity { get; set; }

        [DataMember]
        public int NewLevel { get; set; }

        public LeveledUpMessage(GameObject entity, int newLevel)
        {
            Validate.Hard.IsNotNullOrDefault(entity, this.GetFormattedNullOrDefaultMessage(entity));
            Validate.Hard.IsTrue(newLevel > 0,
                $"Cannot create a {typeof(LeveledUpMessage)} with a {nameof(newLevel)} of {newLevel}");
            Entity = entity;
            NewLevel = newLevel;
        }
    }
}