using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class PathfindingRequest : Message
    {
        [DataMember]
        public DxVector2 Goal { get; private set; }

        [DataMember]
        public UniqueId Requester { get; private set; }

        [DataMember]
        public DxVector2 Start { get; private set; }

        public PathfindingRequest(DxVector2 start, DxVector2 goal, UniqueId requester)
        {
            Start = start;
            Goal = goal;
            Validate.Hard.IsNotNull(requester);
            Requester = requester;
        }
    }
}