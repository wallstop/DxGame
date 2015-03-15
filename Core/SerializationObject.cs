using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace DXGame.Core
{
    public enum SerializationTypes
    {
        None,
        Binary,
        Text
    }

    public delegate void Serialize<in T>(NetOutgoingMessage message);

    public delegate T Deserialize<out T>(NetIncomingMessage message);


    public class SerializationObject
    {

        public SerializationObject()
        {
        }
    }
}
