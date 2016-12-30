using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    public enum AudioType
    {
        None,
        Music,
        Song
    }

    [Serializable]
    [DataContract]
    public class AudioMessage : Message
    {
        [DataMember]
        public string Asset { get; private set; }

        [DataMember]
        public AudioType Type { get; private set; }

        public AudioMessage(string asset, AudioType type)
        {
            Validate.Hard.IsNotEmpty(asset, () => this.GetFormattedNullOrDefaultMessage(nameof(asset)));
            Asset = asset;
            Validate.Hard.IsNotNullOrDefault(type, () => this.GetFormattedNullOrDefaultMessage(type));
            Type = type;
        }
    }
}