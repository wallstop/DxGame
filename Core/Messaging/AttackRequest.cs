using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Messaging
{
    [DataContract]
    [Serializable]
    public class AttackRequest : Message
    {
        /* Plea for an attack */
    }
}
