using System;
using System.IO;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core
{
    [Serializable]
    [DataContract]
    public abstract class JsonPersistable<T> : IPersistable<T>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [IgnoreDataMember]
        public abstract string Extension { get; }

        [IgnoreDataMember]
        public abstract T Item { get; }

        public T Load(string fileName)
        {
            return StaticLoad(fileName);
        }

        public void Save(string fileName)
        {
            try
            {
                string simpleName = Path.GetFileNameWithoutExtension(fileName);
                string directory = Path.GetDirectoryName(fileName);
                Serializer<T>.WriteToJsonFile(Item, directory + Path.DirectorySeparatorChar + simpleName + Extension);
            }
            catch(Exception e)
            {
                LOG.Error(e, $"Caught unexpected exception while saving {GetType()} {this} at {fileName}");
            }
        }

        public static T StaticLoad(string fileName)
        {
            var settingsAsText = File.ReadAllText(fileName);
            var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
            return Serializer<T>.JsonDeserialize(settingsAsJsonByteArray);
        }
    }
}