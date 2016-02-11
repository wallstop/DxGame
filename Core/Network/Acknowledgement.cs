using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    /**
        <summary>
            Basic building block for TCP-over-UDP transmission.
            Acks the sender of a Message that the message has been sent
        </summary>
    */

    [DataContract]
    [Serializable]
    public class Acknowledgement : NetworkMessage
    {
        public UniqueId AcknowledgeId { get; }

        public Acknowledgement(UniqueId acknowledgedId)
        {
            Validate.IsNotNullOrDefault(acknowledgedId,
                StringUtils.GetFormattedNullOrDefaultMessage(this, acknowledgedId));
            AcknowledgeId = acknowledgedId;
        }
    }
}
