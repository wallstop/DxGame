﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Messaging;

namespace DXGame.Core.Messaging
{
    [Serializable]
    [DataContract]
    public class MapRotationNotification : Message
    {
    }
}
