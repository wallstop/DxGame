using System;
using System.Runtime.Serialization;
using DxCore.Core;

namespace AnimationEditor.Core
{
    [DataContract]
    [Serializable]
    public sealed class AnimationSettings : JsonPersistable<AnimationSettings>
    {
        [DataMember]
        public string ContentDirectory { get; set; }

        [IgnoreDataMember]
        public override string Extension => SettingsExtension;

        [IgnoreDataMember]
        public override AnimationSettings Item => this;

        public static string SettingsExtension => ".settings";
    }
}