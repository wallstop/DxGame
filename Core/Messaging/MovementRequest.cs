using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Utils.Distance;

namespace DXGame.Core.Messaging
{
    public class MovementRequest : Message
    {
        public Direction Direction { get; set; }
    }
}
