using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Settings;
using DxCore.Core.Utils;

namespace AnimationEditorLibrary.Core.Settings
{
    [DataContract]
    [Serializable]
    public class AnimationEditorSettings : AbstractSettings<AnimationEditorSettings>
    {
        [DataMember] private string contentDirectory_;
        public static string AnimationEditorSettingsPath => "AnimationEditorSettings.json";

        [IgnoreDataMember]
        public string ContentDirectory
        {
            get { return contentDirectory_; }
            set
            {
                string oldValue = contentDirectory_;
                contentDirectory_ = value;
                if(!Objects.Equals(oldValue, value))
                {
                    Uri newContentDirectory = new Uri(value);
                    Uri relativeToNew = new Uri(Environment.CurrentDirectory).MakeRelativeUri(newContentDirectory);
                    //DxGame.Instance.Content.RootDirectory = relativeToNew.ToString();
                }
            }
        }

        public override AnimationEditorSettings CurrentSettings => this;
        public override AnimationEditorSettings DefaultSettings => new AnimationEditorSettings();

        public override string Path => AnimationEditorSettingsPath;

        public override string ToString() => this.ToJson();
    }
}