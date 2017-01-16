using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Settings;

namespace AnimationEditorLibrary.Core.Settings
{
    [DataContract]
    [Serializable]
    public class AnimationEditorSettings : AbstractSettings<AnimationEditorSettings>
    {
        public static string AnimationEditorSettingsPath => "AnimationEditorSettings.json";

        [DataMember]
        public string ContentDirectory { get; set; }

        public override AnimationEditorSettings CurrentSettings => this;
        public override AnimationEditorSettings DefaultSettings => new AnimationEditorSettings();

        public override string Path => AnimationEditorSettingsPath;

        public override string ToString() => this.ToJson();
    }
}