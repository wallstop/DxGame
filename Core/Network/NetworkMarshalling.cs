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
            NetworkMarshaller<Component>.RegisterReader(ReadComponent);
            NetworkMarshaller<Vector2>.RegisterReader(ReadVector2);
            NetworkMarshaller<Vector2>.RegisterWriter(Write);
        }

        public static void Init()
        {
            
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

        public static void Write(Component component, NetOutgoingMessage message)
        {
            NetworkUtils.WriteTypeTo(component, message);
            message.WriteAllFields(component);
            message.WriteAllProperties(component);
        }

        public static void Write(DrawableComponent drawableComponent, NetOutgoingMessage message)
        {
            NetworkUtils.WriteTypeTo(drawableComponent, message);
            message.WriteAllFields(drawableComponent);
            message.WriteAllProperties(drawableComponent);
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

        public static void Write(Vector2 vector, NetOutgoingMessage message)
        {
            message.Write(vector.X);
            message.Write(vector.Y);
        }

        public static Vector2 ReadVector2(NetIncomingMessage message)
        {
            return new Vector2 {X = message.ReadFloat(), Y = message.ReadFloat()};
        }
    }
}