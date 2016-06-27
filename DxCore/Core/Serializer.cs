using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using DxCore.Core.Utils;

namespace DxCore.Core
{
    public static class Serializer<T>
    {
        internal static readonly Encoding Encoding = Encoding.Default;

        private static DataContractJsonSerializer JsonSerializer => new DataContractJsonSerializer(typeof(T),
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });

        // TODO: Move all of these to thread-local storage

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
                DataContractJsonSerializer serializer = JsonSerializer;
                serializer.WriteObject(memoryStream, input);
                return memoryStream.ToArray();
            }
        }

        public static T JsonDeserialize(byte[] data)
        {
            using(MemoryStream memoryStream = new MemoryStream(data))
            {
                /* TODO: Use Global instance? */
                DataContractJsonSerializer deserializer = JsonSerializer;
                memoryStream.Position = 0;
                return (T) deserializer.ReadObject(memoryStream);
            }
        }

        public static T JsonDeserialize(string data)
        {
            return JsonDeserialize(data.GetBytes());
        }

        public static T ReadFromJsonFile(string path)
        {
            var settingsAsText = File.ReadAllText(path, Encoding);
            var settingsAsJsonByteArray = settingsAsText.GetBytes();
            return JsonDeserialize(settingsAsJsonByteArray);
        }

        public static void WriteToJsonFile(T input, string path)
        {
            string jsonAsText = input.ToJson();
            File.WriteAllText(path, jsonAsText);
        }
    }

    public static class SerializerExtensions
    {
        public static string ToJson<T>(this T input)
        {
            byte[] json = Serializer<T>.JsonSerialize(input);
            string jsonAsText = Serializer<T>.Encoding.GetString(json);
            return jsonAsText;
        }

        public static T Parse<T>(string toParse) => Serializer<T>.JsonDeserialize(toParse);
    }
}