using System;
using System.IO;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.Settings
{
    [DataContract]
    [Serializable]
    public abstract class AbstractSettings<T> : IPersistable<T> where T : IPersistable<T>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public abstract string Path { get; }
        public abstract T DefaultSettings { get; }
        public abstract T CurrentSettings { get; }

        public T Load(string fileName)
        {
            T loadedSettings;
            try
            {
                var settingsAsText = File.ReadAllText(Path);
                var settingsAsJsonByteArray = settingsAsText.GetBytes();
                loadedSettings = Serializer<T>.JsonDeserialize(settingsAsJsonByteArray);
            }
            catch(Exception e)
            {
                LOG.Error(e, $"Caught unexpected exception while loading settings file for {typeof(T)} at {Path}");
                loadedSettings = DefaultSettings;
                DefaultSettings.Save(fileName);
            }

            CopySettings(loadedSettings);
            return loadedSettings;
        }

        public void Save(string fileName)
        {
            try
            {
                Serializer<T>.WriteToJsonFile(CurrentSettings, Path);
            }
            catch(Exception e)
            {
                LOG.Error(e, $"Caught unexpected exception while saving {typeof(T)} {this} at {Path}");
            }
        }

        public void Save()
        {
            Save(Path);
        }

        public T Load()
        {
            return Load(Path);
        }

        protected virtual void CopySettings(T other)
        {
            Validate.Hard.IsNotNull(other, $"Cannot copy Settings for a null {GetType()}");
            this.MapAllFieldsFrom(other);
        }
    }
}