﻿using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Messaging.Game
{
    [DataContract]
    [Serializable]
    public class TimeSkewRequest : Message
    {
        [DataMember]
        public double OffsetMilliseconds { get; private set; }

        public TimeSkewRequest(double offsetMilliseconds)
        {
            OffsetMilliseconds = offsetMilliseconds;
        }
    }
}