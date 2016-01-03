using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced.Triggers
{
    [DataContract]
    [Serializable]
    public class EntitySpawnObserver : TriggerComponent
    {
        private EntitySpawnObserver(Trigger trigger, Action action, TimeSpan checkFrequency = new TimeSpan()) : base(trigger, action, checkFrequency)
        {
        }

        public class Builder : IBuilder<EntitySpawnObserver>
        {
            private ManagerComponent manager_;
            private List<Spawner> spawners_ = new List<Spawner>();
            
            public Builder WithManager() 

            public EntitySpawnObserver Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}
