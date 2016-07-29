using System;
using System.Runtime.Serialization;

namespace BabelUILibrary.Core
{
    [Serializable]
    [DataContract]
    public struct Resolution
    {
        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        public override string ToString() => Width + " x " + Height;
    }
}
