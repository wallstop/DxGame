using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Properties;
using NLog;

namespace DXGame.TowerGame.Items
{
    [DataContract]
    [Serializable]
    public class Gram : ItemComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public Gram(SpatialComponent spatial)
            : base(spatial)
        {
        }

        protected override void HandleEnvironmentInteraction(EnvironmentInteractionMessage environmentInteraction)
        {
            throw new NotImplementedException();
        }

        // TODO private PropertyMutator<int> GramDamageBuff
    }
}
