using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using DXGame.Core.Utils;
using log4net;

namespace DXGame.Core.Settings
{
    [DataContract]
    [Serializable]
    public abstract class AbstractSettings<T> : IPersistable<T> where T : IPersistable<T>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (AbstractSettings<T>));
        public abstract string Path { get; }
        public abstract T DefaultSettings { get; }
        public abstract T CurrentSettings { get; }

        public T Load()
        {
            T loadedSettings;
            try
            {
                var settingsAsText = File.ReadAllText(Path);
                var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
                loadedSettings =
                    Serializer<T>.JsonDeserialize(settingsAsJsonByteArray);
            }
            catch (Exception e)
            {
                LOG.Error($"Caught unexpected exception while loading settings file for {typeof (T)} at {Path}",
                    e);
                loadedSettings = DefaultSettings;
                DefaultSettings.Save();
            }

            CopySettings(loadedSettings);
            return loadedSettings;
        }

        public void Save()
        {
            try
            {
                var json = Serializer<T>.JsonSerialize(CurrentSettings);
                var jsonAsText = Encoding.Default.GetString(json);
                File.WriteAllText(Path, jsonAsText);
            }
            catch (Exception e)
            {
                LOG.Error($"Caught unexpected exception while saving {typeof (T)} {this} at {Path}", e);
            }
        }

        protected abstract void CopySettings(T other);
    }
}