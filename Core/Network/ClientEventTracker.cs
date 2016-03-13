using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Input;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core.Network
{
    
    public class ClientEventTracker
    {
        public ServerEventTracker ServerEventTracker { get; }

        /* TODO: Should we decouple this and send commands? Probably */
        public void ReceiveEvents(List<Commandment> clientKeyboardEvents)
        {
            
        }

        public ClientEventTracker(ServerEventTracker serverEventTracker)
        {
            Validate.IsNotNullOrDefault(serverEventTracker, StringUtils.GetFormattedNullOrDefaultMessage(this, serverEventTracker));
            ServerEventTracker = serverEventTracker;
        }
    }
}
