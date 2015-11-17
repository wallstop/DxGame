using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Map
{
    [Serializable]
    [DataContract]
    public class MapPiece
    {
        [DataMember]
        public List<Platform> Platforms { get; } = new List<Platform>();

        [DataMember]
        public string Asset { get; set; }
    } 
    
}
