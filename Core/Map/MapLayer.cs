using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using ProtoBuf;

namespace DXGame.Core.Map
{
    /**
        <summary>
            Simple data-holder for a Map file (image) and Layer that it should be drawn to
        </summary>
    */

    [DataContract]
    [Serializable]
    public class MapLayer
    {
        public static readonly int DEFAULT_LAYER = 0;

        [DataMember]
        public string Asset { get; set; }

        [DataMember]
        public int Layer { get; set; }

        public MapLayer(string asset) : this(asset, DEFAULT_LAYER) {}

        public MapLayer(string asset, int layer)
        {
            Validate.IsNotNullOrDefault(asset, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(asset)));
            Asset = asset;
            Layer = layer;
        }
    }
}