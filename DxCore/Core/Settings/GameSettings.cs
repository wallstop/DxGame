using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Settings
{
    /*
        This class will hold all of the Game-specific options like Keybinds, screen resolution, graphics fidelity, etc

        Should be easily serializable as a JSON file.
    */

    [DataContract]
    [Serializable]
    public class GameSettings : AbstractSettings<GameSettings>
    {
        [IgnoreDataMember]
        public int ScreenHeight => VideoSettings.ScreenHeight;

        [IgnoreDataMember]
        public int ScreenWidth => VideoSettings.ScreenWidth;

        [DataMember]
        public VideoSettings VideoSettings { get; private set; }

        [IgnoreDataMember]
        public DxRectangle ScreenRegion => new DxRectangle(0, 0, ScreenWidth, ScreenHeight);

        public static GameSettings DefaultGameSettings
            => new GameSettings {VideoSettings = VideoSettings.DefaultVideoSettings};

        public override GameSettings DefaultSettings => DefaultGameSettings;
        public override GameSettings CurrentSettings => this;
        public override string Path => GameSettingsPath;
        public static string GameSettingsPath => "Settings.json";

        public override string ToString() => this.ToJson();
    }
}