using System;
using System.Runtime.Serialization;

namespace DxCore.Core.Messaging
{
    /**
        <summary> 
            Answers the question "Should the recipient 'accept' the damage that I am about to deal? If so, how much damage should I deal?". 
            Imagined uses of this function included checking for same-team-ness to avoid friendly fire.

            If the answer is "yes, I should be doing damage", this function will result in {True, (how much damage)}.
            If the answer is "no, I shouldn't be doing damage", this function will result in {False, undefined}
        </summary>
    */

    public delegate Tuple<bool, double> DamageCheck(GameObject source, GameObject recipient);

    [Serializable]
    [DataContract]
    public class DamageMessage : Message, ITargetedMessage
    {
        /**
            <summary> The GameObject that the Damage originates from. </summary>
        */

        [DataMember]
        public GameObject Source { get; set; }

        [DataMember]
        public DamageCheck DamageCheck { get; set; }

        [DataMember]
        public UniqueId Target { get; set; }
    }
}