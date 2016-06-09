using System;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;

namespace DxCore.Core.Messaging.Entity
{
    /**
        <summary> 
            Meant to serve as a notifier to those who care about a particular entity death 
        </summary>
    */

    [Serializable]
    [DataContract]
    public class EntityDeathMessage : Message
    {
        [DataMember]
        public GameObject Entity { get; set; }
    }
}
