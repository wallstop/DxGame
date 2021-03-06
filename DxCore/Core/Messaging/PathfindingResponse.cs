﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Map;
using WallNetCore.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public sealed class PathfindingResponse : Message, ITargetedMessage
    {
        [DataMember]
        public List<NavigableMeshNode> Path { get; private set; }

        public PathfindingResponse(List<NavigableMeshNode> path, UniqueId target)
        {
            Validate.Hard.IsNotNull(path);
            Path = path;
            Validate.Hard.IsNotNull(target);
            Target = target;
        }

        [DataMember]
        public UniqueId Target { get; private set; }
    }
}