using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    public static class NetworkMarshalling
    {
        static NetworkMarshalling()
        {

            NetworkMarshaller<int>.RegisterSerializerAndDeserializer(SerializeInt32, DeserializeInt32);
            NetworkMarshaller<Int32>.RegisterSerializerAndDeserializer(SerializeInt32, DeserializeInt32);
            NetworkMarshaller<short>.RegisterSerializerAndDeserializer(SerializeInt16, DeserializeInt16);
            NetworkMarshaller<Int16>.RegisterSerializerAndDeserializer(SerializeInt16, DeserializeInt16);
            NetworkMarshaller<byte>.RegisterSerializerAndDeserializer(SerializeByte, DeserializeByte);
            NetworkMarshaller<Byte>.RegisterSerializerAndDeserializer(SerializeByte, DeserializeByte);
            NetworkMarshaller<sbyte>.RegisterSerializerAndDeserializer(SerializeSbyte, DeserializeSbyte);
            NetworkMarshaller<SByte>.RegisterSerializerAndDeserializer(SerializeSbyte, DeserializeSbyte);
            NetworkMarshaller<bool>.RegisterSerializerAndDeserializer(SerializeBool, DeserializeBool);
            NetworkMarshaller<Boolean>.RegisterSerializerAndDeserializer(SerializeBool, DeserializeBool);
            NetworkMarshaller<float>.RegisterSerializerAndDeserializer(SerializeFloat, DeserializeFloat);
            NetworkMarshaller<double>.RegisterSerializerAndDeserializer(SerializeDouble, DeserializeDouble);
            NetworkMarshaller<Double>.RegisterSerializerAndDeserializer(SerializeDouble, DeserializeDouble);
            NetworkMarshaller<long>.RegisterSerializerAndDeserializer(SerializeLong, DeserializeLong);
            NetworkMarshaller<Int64>.RegisterSerializerAndDeserializer(SerializeLong, DeserializeLong);
            NetworkMarshaller<uint>.RegisterSerializerAndDeserializer(SerializeUint, DeserializeUint);
            NetworkMarshaller<UInt32>.RegisterSerializerAndDeserializer(SerializeUint, DeserializeUint);
            NetworkMarshaller<ushort>.RegisterSerializerAndDeserializer(SerializeUint16, DeserializeUint16);
            NetworkMarshaller<UInt16>.RegisterSerializerAndDeserializer(SerializeUint16, DeserializeUint16);
            NetworkMarshaller<string>.RegisterSerializerAndDeserializer(SerializeString, DeserializeString);
            NetworkMarshaller<String>.RegisterSerializerAndDeserializer(SerializeString, DeserializeString);

            NetworkMarshaller<Vector2>.RegisterSerializerAndDeserializer(SerializeVector2, DeserializeVector2);
            NetworkMarshaller<Type>.RegisterSerializerAndDeserializer(SerializeType, DeserializeType);
        }

        public static void Init()
        {
            /*
                We define an empty Init function so the static constructor above gets called and registers all of our types :^)
            */
        }

        public static void SerializeIEnumerable<U, T>(U collection, NetOutgoingMessage message) where U : IEnumerable<T>
            where T : INetworkSerializable
        {
            var networkSerializables = collection as IList<T> ?? collection.ToList();

            if (collection == null || !networkSerializables.Any())
            {
                NetworkMarshaller<int>.Serialize(0, message);
                return;
            }

            NetworkMarshaller<int>.Serialize(networkSerializables.Count(), message);
            foreach (var networkSerializable in networkSerializables)
            {
                networkSerializable.SerializeTo(message);
            }
        }

        /*
            TODO: See if this works. Likely, we need some kind of higher-level object that knows to automagically 
            write out types & instantiate stuff so we get proper virtual methods going
        */

        public static List<T> DeserializeIEnumerable<T>(NetIncomingMessage message) where T : INetworkSerializable
        {
            var collectionSize = NetworkMarshaller<int>.Deserialize(message);
            var serializables = new List<T>(collectionSize);
            for (var i = 0; i < collectionSize; ++i)
            {
                var serializable = NetworkMarshaller<T>.Deserialize(message);
                serializables.Add(serializable);
            }

            return serializables;
        }

        public static void SerializeDictionary<T, K, V>(T input, NetOutgoingMessage message) where T : IDictionary<K, V>
            where K : INetworkSerializable where V : INetworkSerializable
        {
            if (input == null || !input.Any())
            {
                NetworkMarshaller<int>.Serialize(0, message);
                return;
            }

            NetworkMarshaller<int>.Serialize(input.Count, message);
            foreach (var keyValuePair in input)
            {
                K key = keyValuePair.Key;
                V value = keyValuePair.Value;
                key.SerializeTo(message);
                value.SerializeTo(message);
            }
        }

        public static Dictionary<K, V> DeserializeDictionary<K, V>(NetIncomingMessage message)
            where K : INetworkSerializable where V : INetworkSerializable
        {
            var dictionarySize = NetworkMarshaller<int>.Deserialize(message);
            Dictionary<K, V> output = new Dictionary<K, V>();
            for (int i = 0; i < dictionarySize; ++i)
            {
                K key = NetworkMarshaller<K>.Deserialize(message);
                V value = NetworkMarshaller<V>.Deserialize(message);
                output[key] = value;
            }
            return output;
        }

        public static void SerializeVector2(Vector2 vector, NetOutgoingMessage message)
        {
            NetworkMarshaller<float>.Serialize(vector.X, message);
            NetworkMarshaller<float>.Serialize(vector.Y, message);
        }

        public static Vector2 DeserializeVector2(NetIncomingMessage message)
        {
            return new Vector2 { X = NetworkMarshaller<float>.Deserialize(message), Y = NetworkMarshaller<float>.Deserialize(message) };
        }

        public static Int32 DeserializeInt32(NetIncomingMessage message)
        {
            return message.ReadInt32();
        }

        public static void SerializeInt32(Int32 input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static Int16 DeserializeInt16(NetIncomingMessage message)
        {
            return message.ReadInt16();
        }

        public static void SerializeInt16(Int16 input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static UInt16 DeserializeUint16(NetIncomingMessage message)
        {
            return message.ReadUInt16();
        }

        public static void SerializeUint16(UInt16 input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static bool DeserializeBool(NetIncomingMessage message)
        {
            return message.ReadBoolean();
        }

        public static void SerializeBool(bool input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static double DeserializeDouble(NetIncomingMessage message)
        {
            return message.ReadDouble();
        }

        public static void SerializeDouble(double input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static float DeserializeFloat(NetIncomingMessage message)
        {
            return message.ReadFloat();
        }

        public static void SerializeFloat(float input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static byte DeserializeByte(NetIncomingMessage message)
        {
            return message.ReadByte();
        }

        public static void SerializeByte(byte input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static sbyte DeserializeSbyte(NetIncomingMessage message)
        {
            return message.ReadSByte();
        }

        public static void SerializeSbyte(sbyte input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static void SerializeLong(long input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static long DeserializeLong(NetIncomingMessage message)
        {
            return message.ReadVariableInt64();
        }

        public static void SerializeUint(uint input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static uint DeserializeUint(NetIncomingMessage message)
        {
            return message.ReadUInt32();
        }

        public static void SerializeString(string input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static string DeserializeString(NetIncomingMessage message)
        {
            return message.ReadString();
        }

        public static Type DeserializeType(NetIncomingMessage message)
        {
            var typeString = message.ReadString();
            return Type.GetType(typeString, true);
        }

        public static void SerializeType(Type input, NetOutgoingMessage message)
        {
            message.Write(input.ToString());
        }
    }
}