using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class EnvironmentInteractionMessage : Message
    {
        [DataMember]
        public GameObject Source { get; set; }

        [DataMember]
        public DxGameTime Time { get; set; }
    }
}
