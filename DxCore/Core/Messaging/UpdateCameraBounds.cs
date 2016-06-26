using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class UpdateCameraBounds : Message
    {
        [DataMember]
        public DxRectangle Bounds { get; }

        public UpdateCameraBounds(DxRectangle bounds)
        {
            Validate.Hard.IsNotNullOrDefault(bounds);
            Bounds = bounds;
        }
    }
}
