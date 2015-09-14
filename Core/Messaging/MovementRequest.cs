using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class MovementRequest : Message
    {
        [DataMember]
        public Direction Direction { get; set; }
    }
}
