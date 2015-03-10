using System;
using Lidgren.Network;

namespace DXGame.Core.Network
{
    public delegate void Serialize<in T>(T input, NetOutgoingMessage message);

    public delegate T Deserialize<out T>(NetIncomingMessage message);

    public class NetworkMarshaller<T>
    {
        private Serialize<T> serializer_;
        private Deserialize<T> deserializer_;

        private static readonly Lazy<NetworkMarshaller<T>> singleton_ =
            new Lazy<NetworkMarshaller<T>>(() => new NetworkMarshaller<T>());

        private static NetworkMarshaller<T> Instance
        {
            get { return singleton_.Value; }
        }

        internal NetworkMarshaller()
        {
        }

        public static void RegisterSerializer(Serialize<T> serializer)
        {
            if (Instance.serializer_ != null && serializer != Instance.serializer_)
            {
                throw new ArgumentException(String.Format("Already registered a serializer for {0}", typeof (T)));
            }
            Instance.serializer_ = serializer;
        }

        public static void RegisterDeserializer(Deserialize<T> deserializer)
        {
            if (Instance.deserializer_ != null && Instance.deserializer_ != deserializer)
            {
                throw new ArgumentException(String.Format("Already registered a deserializer for {0}", typeof (T)));
            }
            Instance.deserializer_ = deserializer;
        }

        public static void RegisterSerializerAndDeserializer(Serialize<T> serializer, Deserialize<T> deserializer)
        {
            RegisterSerializer(serializer);
            RegisterDeserializer(deserializer);
        }

        public static T Deserialize(NetIncomingMessage message)
        {
            return Instance.deserializer_(message);
        }

        public static void Serialize(T instance, NetOutgoingMessage message)
        {
            Instance.serializer_(instance, message);
        }
    }
}