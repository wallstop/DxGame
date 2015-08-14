﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using DXGame.Core.Utils;

namespace DXGame.Core
{
    public static class Serializer<T>
    {
        // TODO: Move all of these to thread-local storage
        public static byte[] BinarySerialize(T input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T BinaryDeserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                memoryStream.Position = 0;
                return (T) binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static byte[] JsonSerialize(T input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
                serializer.WriteObject(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T JsonDeserialize(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof (T));
                memoryStream.Position = 0;
                return (T) deserializer.ReadObject(memoryStream);
            }
        }

        public static T JsonDeserialize(string data)
        {
            return JsonDeserialize(StringUtils.GetBytes(data));
        }

        public static T ReadFromJsonFile(string path)
        {
            var settingsAsText = File.ReadAllText(path);
            var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
            return JsonDeserialize(settingsAsJsonByteArray);
        }

        public static void WriteToJsonFile(T input, string path)
        {
            var json = JsonSerialize(input);
            var jsonAsText = Encoding.Default.GetString(json);
            File.WriteAllText(path, jsonAsText);
        }
    }
}