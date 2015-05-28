using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using DXGame.Core.Utils;

namespace DXGame.Core
{
    public static class Serializer<T>
    {
        public static byte[] BinarySerialize(T input)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, input);
            return memoryStream.ToArray();
        }

        public static T BinaryDeserialize(byte[] data)
        {
            if (data.Length < 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"Can not deserialize a byte array of length {data.Length}");
            }

            MemoryStream memoryStream = new MemoryStream(data);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            memoryStream.Position = 0;
            return (T) binaryFormatter.Deserialize(memoryStream);
        }

        public static byte[] JsonSerialize(T input)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            serializer.WriteObject(memoryStream, input);
            return memoryStream.ToArray();
        }

        public static T JsonDeserialize(byte[] data)
        {
            if (data.Length < 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"Can not deserialize a JSON byte array of length {data.Length}");
            }

            MemoryStream memoryStream = new MemoryStream(data);
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof (T));
            memoryStream.Position = 0;
            return (T) deserializer.ReadObject(memoryStream);
        }

        public static T JsonDeserialize(string data)
        {
            return JsonDeserialize(StringUtils.GetBytes(data));
        }
    }
}