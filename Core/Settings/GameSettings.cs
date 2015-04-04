using System;
using System.IO;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using log4net;

namespace DXGame.Core.Settings
{
    /*
        This class will hold all of the Game-specific options like Keybings, screen resolution, graphics fidelity, etc
    */

    [DataContract]
    public class GameSettings : IPersistable<GameSettings>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (GameSettings));
        [DataMember] public int ScreenHeight;
        [DataMember] public int ScreenWidth;

        public static GameSettings DefaultSettings
        {
            get { return new GameSettings {ScreenHeight = 720, ScreenWidth = 1280}; }
        }

        public static string Path
        {
            get { return "Settings.json"; }
        }

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
                LOG.Error(String.Format("Caught unexpected exception while saving {0}", this), e);
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
                LOG.Error(String.Format("Caught unexpected exception while loading settings file"),
                    e);
                loadedSettings = DefaultSettings;
                DefaultSettings.Save();
            }

            // Copy the loaded object in, then throw it away
            ScreenHeight = loadedSettings.ScreenHeight;
            ScreenWidth = loadedSettings.ScreenWidth;
            return this;
        }

        public override string ToString()
        {
            var json = Serializer<GameSettings>.JsonSerialize(this);
            var jsonAsText = StringUtils.GetString(json);
            return jsonAsText;
        }
    }
}