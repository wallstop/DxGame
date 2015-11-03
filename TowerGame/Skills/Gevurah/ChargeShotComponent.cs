using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;

namespace DXGame.TowerGame.Skills.Gevurah
{
    [Serializable]
    [DataContract]
    public class ChargeShotComponent : Component
    {
        private static readonly TimeSpan MAX_CHARGE_TIME = TimeSpan.FromSeconds(5);

        [DataMember]
        private TimeSpan ChargeTime { get; set; }

        [DataMember]
        private bool Charging { get; set; }

        protected override void Update(DxGameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
