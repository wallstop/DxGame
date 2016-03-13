using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class EnvironmentInteractionMessage : Message
    {
        [DataMember]
        public GameObject Source { get; set; }
    }
}
