using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
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
        [DataMember] private uint colorAsInt_;

        // TODO: Cache created color

        [IgnoreDataMember]
        public Color Color
        {
            get
            {
                Color color = new Color
                {
                    A = colorAsInt_.A(),
                    B = colorAsInt_.B(),
                    G = colorAsInt_.C(),
                    R = colorAsInt_.D()
                };
                return color;
            }
            set { colorAsInt_ = value.PackedValue; }
        }

        public DxColor(Color color)
        {
            colorAsInt_ = color.PackedValue;
        }
    }
}