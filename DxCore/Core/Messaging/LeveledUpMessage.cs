using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

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
            Validate.IsNotNullOrDefault(entity, StringUtils.GetFormattedNullOrDefaultMessage(this, entity));
            Validate.IsTrue(newLevel > 0,
                $"Cannot create a {typeof(LeveledUpMessage)} with a {nameof(newLevel)} of {newLevel}");
            Entity = entity;
            NewLevel = newLevel;
        }
    }
}