using System;
using System.Runtime.Serialization;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Triggers;
using DXGame.Main;
using DXGame.TowerGame.Messaging;

namespace DXGame.TowerGame.Components.Waves
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
            TimedTriggeredActionComponent waveEmitter = new TimedTriggeredActionComponent(TimeSpan.FromMinutes(30), waveDelay,
                waveCounter.EmitWaveNotification);

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
                DxGame.Instance.BroadcastMessage(newWaveMessage);
            }
        }
    }
}