﻿using System;
using System.IO;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Settings
{
    [DataContract]
    [Serializable]
    public abstract class AbstractSettings<T> : IPersistable<T> where T : IPersistable<T>
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public abstract T CurrentSettings { get; }
        public abstract T DefaultSettings { get; }

        public abstract string Path { get; }

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
                Logger.Error(e, $"Caught unexpected exception while loading settings file for {typeof(T)} at {Path}");
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
                Logger.Error(e, $"Caught unexpected exception while saving {typeof(T)} {this} at {Path}");
            }
        }

        public T Load()
        {
            return Load(Path);
        }

        public void Save()
        {
            Save(Path);
        }

        protected virtual void CopySettings(T other)
        {
            Validate.Hard.IsNotNull(other, () => $"Cannot copy Settings for a null {GetType()}");
            this.MapAllFieldsFrom(other);
        }
    }
}