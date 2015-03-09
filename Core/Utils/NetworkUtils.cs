using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Network;
using Lidgren.Network;

namespace DXGame.Core.Utils
{
    public static class NetworkUtils
    {

        public static void WriteTypeTo<T>(NetOutgoingMessage message) where T: class
        {
            WriteTypeTo(typeof(T), message);
        }

        public static void WriteTypeTo(Type type, NetOutgoingMessage message)
        {
            message.Write(type.ToString());
        }

        public static void WriteTypeTo(Object instance, NetOutgoingMessage message)
        {
            WriteTypeTo(instance.GetType(), message);
        }

        public static T ReadTypeFrom<T>(NetIncomingMessage message) where T : class
        {
            var typeString = message.ReadString();
            var type = Type.GetType(typeString, true);
            return (T)Activator.CreateInstance(type);
        }
    }
}
