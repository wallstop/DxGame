using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Messaging
{
    /**
        <summary> Meant to serve as a notifier to those who care about a particular entity death </summary>
    */
    [Serializable]
    [DataContract]
    public class EntityDeathMessage : Message
    {
        [DataMember]
        public GameObject Entity { get; set; }
    }
}
