using System;
using System.IO;
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
    public class GameSettings : IPersistable<GameSettings>
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

        public static GameSettings DefaultSettings
            => new GameSettings {ScreenHeight = 720, ScreenWidth = 1280, HudScale = Scale.Medium};

        public static string Path => "Settings.json";

        public void Save()
        {
            try
            {
                var json = Serializer<GameSettings>.JsonSerialize(this);
                var jsonAsText = System.Text.Encoding.Default.GetString(json);
                File.WriteAllText(Path, jsonAsText);
            }
            catch (Exception e)
            {
                LOG.Error($"Caught unexpected exception while saving {this}", e);
            }
        }

        public GameSettings Load()
        {
            GameSettings loadedSettings;
            try
            {
                var gameSettingsAsText = File.ReadAllText(Path);
                var gameSettingsAsJsonByteArray = StringUtils.GetBytes(gameSettingsAsText);
                loadedSettings =
                    Serializer<GameSettings>.JsonDeserialize(gameSettingsAsJsonByteArray);
            }
            catch (Exception e)
            {
                LOG.Error("Caught unexpected exception while loading settings file",
                    e);
                loadedSettings = DefaultSettings;
                DefaultSettings.Save();
            }

            CopySettings(loadedSettings);
            return this;
        }

        private void CopySettings(GameSettings other)
        {
            // Assume other GameSettings is non-null
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