using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using NLog;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapDescriptor : IPersistable<MapDescriptor>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public static string Extension => ".mdtr";

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public List<Platform> Platforms { get; set; } = new List<Platform>();

        [DataMember]
        public DxRectangle Size { get; set; }

        public MapDescriptor Load(string fileName)
        {
            var settingsAsText = File.ReadAllText(fileName);
            var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
            return Serializer<MapDescriptor>.JsonDeserialize(settingsAsJsonByteArray);
        }

        public void Save(string fileName)
        {
            try
            {
                string simpleName = Path.GetFileNameWithoutExtension(fileName);
                string directory = Path.GetDirectoryName(fileName);
                Serializer<MapDescriptor>.WriteToJsonFile(this,
                    directory + Path.DirectorySeparatorChar + simpleName + Extension);
            }
            catch (Exception e)
            {
                LOG.Error(e,
                    $"Caught unexpected exception while saving {typeof (MapDescriptor)} {this} at {Path.GetFileNameWithoutExtension(Asset) + Extension}");
            }
        }
    }
}