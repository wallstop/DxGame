using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using WallNetCore.Validate;

namespace DxCore.Core.State
{
    /**
        <summary>
            Tiny config specifying how state's should perform their update. 
            Values should be deterministic in terms of the StateMachine that produces it. 
            (ie, tied to a similar config in a StateMachine)
        </summary>
    */

    [Serializable]
    [DataContract]
    public struct StateUpdateConfig
    {
        [DataMember]
        public DxGameTime GameTime { get; private set; }

        [DataMember]
        public bool LoggingEnabled { get; private set; }

        public StateUpdateConfig(DxGameTime gameTime, bool loggingEnabled = false)
        {
            Validate.Hard.IsNotNull(gameTime);
            GameTime = gameTime;
            LoggingEnabled = loggingEnabled;
        }
    }
}