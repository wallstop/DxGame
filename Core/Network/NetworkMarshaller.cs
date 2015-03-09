using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Network
{
    public delegate void Write<in T>(T input, NetOutgoingMessage message);

    public delegate T Read<out T>(NetIncomingMessage message);

    public class NetworkMarshaller<T>
    {
        private Write<T> writer_;
        private Read<T> reader_;

        private static readonly Lazy<NetworkMarshaller<T>> singleton_ = new Lazy<NetworkMarshaller<T>>(() => new NetworkMarshaller<T>());

        private static NetworkMarshaller<T> Instance
        {
            get { return singleton_.Value; }
        }

        internal NetworkMarshaller()
        {
        }

        public static void RegisterReader(Read<T> reader)
        {
            if (Instance.reader_ != null)
            {
                throw new ArgumentException(String.Format("Already registered a reader for {0}", typeof (T)));
            }
            Instance.reader_ = reader;
        }

        public static void RegisterWriter(Write<T> writer)
        {
            if (Instance.writer_ != null)
            {
                throw new ArgumentException(String.Format("Already registered a writer for {0}", typeof (T)));
            }
            Instance.writer_ = writer;
        }

        public static T Read(NetIncomingMessage message)
        {
            return Instance.reader_(message);
        }

        public static void Write(T instance, NetOutgoingMessage message)
        {
            Instance.writer_(instance, message);
        }
    }
}