using System;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Messaging
{
    /**
        <summary>
            Alerts game components that a new "wave" of monsters is spawning
        </summary>
    */

    [DataContract]
    [Serializable]
    public class NewWaveMessage : Message
    {
        [DataMember]
        public int WaveNumber { get; }

        public override bool Global => true;

        public NewWaveMessage(int waveNumber)
        {
            Validate.IsTrue(waveNumber > 0, $"Expected {nameof(waveNumber)} to be positive, but was {waveNumber}");
            WaveNumber = waveNumber;
        }
    }
}