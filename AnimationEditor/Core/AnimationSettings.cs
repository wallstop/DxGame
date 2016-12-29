using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Settings;

namespace AnimationEditor.Core
{
    [DataContract]
    [Serializable]
    public sealed class AnimationSettings : AbstractSettings<AnimationSettings>
    {
        public static string AnimationSettingsPath => "AnimationSettings.json";

        [DataMember]
        public string ContentDirectory { get; set; }

        public override AnimationSettings CurrentSettings => this;

        public static AnimationSettings DefaultAnimationSettings
            => new AnimationSettings {ContentDirectory = AnimationSettingsPath};

        public override AnimationSettings DefaultSettings => DefaultAnimationSettings;

        public override string Path => AnimationSettingsPath;

        public override string ToString() => this.ToJson();
    }
}