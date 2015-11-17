using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapTile
    {
        [DataMember]
        public DxRectangle Space { get; set; }

        [DataMember]
        public MapPiece MapPiece { get; set; }

        [DataMember]
        public int Layer { get; set; }
    }
}
