using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using log4net;

namespace DXGame.Core.Settings
{
    /*
        This class will hold all of the Game-specific options like Keybings, screen resolution, graphics fidelity, etc

        Should be easily serializable as a JSON file.
    */

    [DataContract]
    [Serializable]
    public class GameSettings : AbstractSettings<GameSettings>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (GameSettings));
        private static readonly double HUD_HEIGHT_SCALE = 0.3;
        private static readonly double HUD_WIDTH_SCALE = 1.0;
        [DataMember] public Scale HudScale;
        [DataMember] public int ScreenHeight;
        [DataMember] public int ScreenWidth;
        public DxRectangle ScreenRegion => new DxRectangle(0, 0, ScreenWidth, ScreenHeight);

        public DxRectangle HudRegion
        {
            get
            {
                double hudScale = ScalarForHud(HudScale);
                int width = (int) (ScreenWidth * hudScale * HUD_WIDTH_SCALE);
                int height = (int) (ScreenHeight * hudScale * HUD_HEIGHT_SCALE);
                int x = (ScreenWidth - width) / 2;
                int y = (ScreenHeight - height);
                return new DxRectangle(x, y, width, height);
            }
        }

        public static GameSettings DefaultGameSettings
            => new GameSettings {ScreenHeight = 720, ScreenWidth = 1280, HudScale = Scale.Medium};

        public override GameSettings DefaultSettings => DefaultGameSettings;
        public override GameSettings CurrentSettings => this;
        public override string Path => GameSettingsPath;
        public static string GameSettingsPath => "Settings.json";

        protected override void CopySettings(GameSettings other)
        {
            Validate.IsNotNullOrDefault(other, $"Cannot copy Settings for a null {nameof(GameSettings)}");
            ScreenHeight = other.ScreenHeight;
            ScreenWidth = other.ScreenWidth;
            HudScale = other.HudScale;
        }

        private static double ScalarForHud(Scale scaleFactor)
        {
            switch (scaleFactor)
            {
                case Scale.Ants:
                    return .1;
                case Scale.Small:
                    return .25;
                default:
                case Scale.Medium:
                    return .3;
                case Scale.Large:
                    return .4;
                case Scale.Monstrous:
                    return .6;
            }
        }

        public override string ToString()
        {
            byte[] json = Serializer<GameSettings>.JsonSerialize(this);
            string jsonAsText = StringUtils.GetString(json);
            return jsonAsText;
        }
    }
}