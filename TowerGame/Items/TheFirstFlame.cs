using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public class TheFirstFlame : ItemComponent
    {
        private static readonly float TRIGGER_THRESHOLD = 0.2f;

        public TheFirstFlame(SpatialComponent spatial) : base(spatial)
        {
            MessageHandler.RegisterMessageHandler<AttackBuilder>(HandleAttackBuilderRequest);
        }

        protected void HandleAttackBuilderRequest(AttackBuilder attackBuilder)
        {
            /*  */
        }

        protected PhysicsMessage Burninator(GameObject source, ICollection<IShape> sourceAttackAreas)
        {
                        float rng = ThreadLocalRandom.Current.NextFloat();
            if(rng > TRIGGER_THRESHOLD)
            {
                return;
            }
        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            throw new NotImplementedException();
        }
    }
}
