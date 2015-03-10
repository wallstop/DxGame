using System;
using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
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
            NetworkMarshaller<Int16>.RegisterSerializerAndDeserializer(SerializeInt16, DeserializeInt16);
            NetworkMarshaller<byte>.RegisterSerializerAndDeserializer(SerializeByte, DeserializeByte);
            NetworkMarshaller<bool>.RegisterSerializerAndDeserializer(SerializeBool, DeserializeBool);
            NetworkMarshaller<float>.RegisterSerializerAndDeserializer(SerializeFloat, DeserializeFloat);
            NetworkMarshaller<double>.RegisterSerializerAndDeserializer(SerializeDouble, DeserializeDouble);
            NetworkMarshaller<long>.RegisterSerializerAndDeserializer(SerializeLong, DeserializeLong);
            NetworkMarshaller<Int64>.RegisterSerializerAndDeserializer(SerializeLong, DeserializeLong);

            NetworkMarshaller<Vector2>.RegisterSerializerAndDeserializer(SerializeVector2, DeserializeVector2);
        }

        public static void Init()
        {
            /*
                We define an empty Init function so the static constructor above gets called and registers all of our types :^)
            */
        }

        public static void Write(List<IGameComponent> components, NetOutgoingMessage message)
        {
            if (components == null || components.Count == 0)
            {
                message.WriteVariableUInt32(0);
                return;
            }

            message.WriteVariableUInt32((uint) components.Count);
            foreach (var gameComponent in components)
            {
                Write(gameComponent, message);
            }
        }

        public static List<IGameComponent> ReadGameComponentList(NetIncomingMessage message)
        {
            uint count = message.ReadUInt32();
            List<IGameComponent> components = new List<IGameComponent>((int) count);
            for (int i = 0; i < count; ++i)
            {
                components.Add(ReadGameComponent(message));
            }

            return components;
        }

        public static void Write(IGameComponent gameComponent, NetOutgoingMessage message)
        {
            var component = gameComponent as Component;
            if (component != null)
            {
                Write(component, message);
            }

            var drawableComponent = gameComponent as DrawableComponent;
            if (drawableComponent != null)
            {
                Write(drawableComponent, message);
            }

            throw new NotImplementedException(String.Format("Currently do not support lidgren I/O for {0}",
                gameComponent.GetType()));
        }

        public static IGameComponent ReadGameComponent(NetIncomingMessage message)
        {
            IGameComponent component = NetworkUtils.ReadTypeFrom<IGameComponent>(message);
            message.ReadAllFields(component);
            message.ReadAllProperties(component);
            return component;
        }

        public static Component ReadComponent(NetIncomingMessage message)
        {
            return (Component) ReadGameComponent(message);
        }

        public static void SerializeVector2(Vector2 vector, NetOutgoingMessage message)
        {
            message.Write(vector.X);
            message.Write(vector.Y);
        }

        public static Vector2 DeserializeVector2(NetIncomingMessage message)
        {
            return new Vector2 {X = message.ReadFloat(), Y = message.ReadFloat()};
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

        public static void SerializeLong(long input, NetOutgoingMessage message)
        {
            message.Write(input);
        }

        public static long DeserializeLong(NetIncomingMessage message)
        {
            return message.ReadVariableInt64();
        }

        public static uint DeserializeUint(NetIncomingMessage message)
        {
            return message.ReadUInt32();
        }
    }
}