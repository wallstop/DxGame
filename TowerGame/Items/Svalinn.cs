using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Items
{
    /**
        <summary>
            Based on preliminary Item designs 2016-01-15

            Svalinn is a defensive item similar to Dota's Mjollnir active - it triggers chain lightening upon taking damage
        </summary>
        <description>
            A shield cold enough to withstand the fierce heat of the sun.
        </description>
    */

    [DataContract]
    [Serializable]
    public class Svalinn : ItemComponent
    {
        private static readonly float TRIGGER_CHANCE = 0.1f;

        [DataMember] private bool bound_;

        public Svalinn(SpatialComponent spatial) : base(spatial)
        {
            bound_ = false;
        }

        protected override void Update(DxGameTime gameTime)
        {
            if(!bound_)
            {
                AttemptToBindDamageListener();
            }
        }

        private void AttemptToBindDamageListener()
        {
            EntityPropertiesComponent entityProperties = Parent.ComponentOfType<EntityPropertiesComponent>();
            entityProperties.EntityProperties.Health.AttachListener(DamageCheckListener);
        }

        private void DamageCheckListener(int previousHealth, int currentHealth)
        {
            if(previousHealth <= currentHealth)
            {
                /* No actual damage taken - we only trigger on damage, so bail */
                return;
            }

            float rng = ThreadLocalRandom.Current.NextFloat();
            if(rng > TRIGGER_CHANCE)
            {
                /* Didn't meat trigger requirements - bail */
                return;
            }

            /* Trigger chain lightening effect */

            // TODO: Chain and stuff (Emit Physics Message -> Damage message -> Repeat)

        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    internal sealed class ChainLightning
    {
        //private static readonly int MAX_CHAINS = 6;

        //public 


    }
}
