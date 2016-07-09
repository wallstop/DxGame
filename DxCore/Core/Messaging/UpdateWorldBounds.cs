using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class UpdateWorldBounds : Message
    {
        [DataMember]
        public DxRectangle Bounds { get; private set; }

        public UpdateWorldBounds(DxRectangle bounds)
        {
            Validate.Hard.IsNotNullOrDefault(bounds);
            Bounds = bounds;
        }
    }
}
