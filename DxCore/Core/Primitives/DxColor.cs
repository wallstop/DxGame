using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Primitives
{
    /**
        <summary>
            Serializable and json-able wrapper around an XNA color
        </summary>
    */

    [Serializable]
    [DataContract]
    public struct DxColor
    {
        [DataMember]
        private uint ColorAsInt { get; set; }

        [IgnoreDataMember] [NonSerialized] private Color cachedColor_;

        // TODO: Cache created color

        [IgnoreDataMember]
        public Color Color => cachedColor_;

        public DxColor(Color color)
        {
            ColorAsInt = color.PackedValue;
            cachedColor_ = new Color {PackedValue = ColorAsInt};
        }

        [OnDeserialized]
        private void OnDeserialized()
        {
            cachedColor_.PackedValue = ColorAsInt;
        }
    }
}