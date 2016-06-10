using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Triggers;
using DxCore.Core.Messaging;
using DXGame.Core;
using DXGame.TowerGame.Messaging;

namespace Babel.Components.Waves
{
    /**
        <summary>
            Wrapper class for generating a GameObject that Emits "NewWave" notifications
        </summary>
    */

    public static class WaveEmitter
    {
        public static GameObject NewWaveEmitter()
        {
            TimeSpan waveDelay = TimeSpan.FromSeconds(5);

            WaveCounter waveCounter = new WaveCounter();
            TimedTriggeredActionComponent waveEmitter = new TimedTriggeredActionComponent(TimeSpan.FromMinutes(30),
                waveDelay, waveCounter.EmitWaveNotification);

            return GameObject.Builder().WithComponent(waveEmitter).Build();
        }

        [DataContract]
        [Serializable]
        internal sealed class WaveCounter
        {
            private int waveNumber_;

            public void EmitWaveNotification()
            {
                ++waveNumber_;
                NewWaveMessage newWaveMessage = new NewWaveMessage(waveNumber_);
                newWaveMessage.Emit();
            }
        }
    }
}