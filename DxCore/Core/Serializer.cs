using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using DXGame.Core.Utils;
using ProtoBuf;
using Salar.Bois;

namespace DXGame.Core
{
    public static class Serializer<T>
    {
        private static readonly Encoding ENCODING = Encoding.Default;

        // TODO: Move all of these to thread-local storage

        public static byte[] NetSerializer(T input)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                NetSerializer.Serializer serializer = new NetSerializer.Serializer(new[] {typeof(T)});
                serializer.Serialize(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T NetDeserialize(byte[] data)
        {
            using(MemoryStream memoryStream = new MemoryStream(data))
            {
                NetSerializer.Serializer deserializer = new NetSerializer.Serializer(new[] {typeof(T)});
                return (T) deserializer.Deserialize(memoryStream);
            }
        }

        public static byte[] SolarBoisSerialize(T input)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                BoisSerializer serializer = new BoisSerializer();
                serializer.Serialize(input, memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static T SolarBoisDeserializer(byte[] data)
        {
            using(MemoryStream memoryStream = new MemoryStream(data))
            {
                BoisSerializer deserializer = new BoisSerializer();
                return deserializer.Deserialize<T>(memoryStream);
            }
        }

        public static byte[] BinarySerialize(T input)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T BinaryDeserialize(byte[] data)
        {
            using(MemoryStream memoryStream = new MemoryStream(data))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                memoryStream.Position = 0;
                return (T) binaryFormatter.Deserialize(memoryStream);
            }
        }

        public static byte[] JsonSerialize(T input)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T JsonDeserialize(byte[] data)
        {
            using(MemoryStream memoryStream = new MemoryStream(data))
            {
                /* TODO: Use Global instance? */
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
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
            var settingsAsText = File.ReadAllText(path, ENCODING);
            var settingsAsJsonByteArray = StringUtils.GetBytes(settingsAsText);
            return JsonDeserialize(settingsAsJsonByteArray);
        }

        public static void WriteToJsonFile(T input, string path)
        {
            var json = JsonSerialize(input);
            var jsonAsText = ENCODING.GetString(json);
            File.WriteAllText(path, jsonAsText);
        }
    }
}