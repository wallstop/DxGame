﻿using System;
using System.IO;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.Settings
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
                var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
                loadedSettings =
                    Serializer<T>.JsonDeserialize(settingsAsJsonByteArray);
            }
            catch (Exception e)
            {
                LOG.Error(e, $"Caught unexpected exception while loading settings file for {typeof (T)} at {Path}"
                    );
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
            catch (Exception e)
            {
                LOG.Error(e, $"Caught unexpected exception while saving {typeof (T)} {this} at {Path}");
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

        protected abstract void CopySettings(T other);
    }
}