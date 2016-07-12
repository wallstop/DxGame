using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class PathFindingRequest : Message
    {
        [DataMember]
        public DxVector2 Start { get; private set; }

        [DataMember]
        public DxVector2 Goal { get; private set; }

        [DataMember]
        public UniqueId Requester { get; private set; }

        public PathFindingRequest(DxVector2 start, DxVector2 goal, UniqueId requester)
        {
            Start = start;
            Goal = goal;
            Validate.Hard.IsNotNull(requester);
            Requester = requester;
        }
    }
}