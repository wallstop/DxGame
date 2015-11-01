using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Entities
{
    /**

        <summary>
            A spawner that repositions the spawn location of every entity to somewhere (hopefully) reachable on the map
        </summary>
    */
    [Serializable]
    [DataContract]
    public class RandomSpawner : Spawner
    {
        protected override DxRectangle SpawnArea {
            get
            {
                MapModel mapModel = DxGame.Instance.Model<MapModel>();
                return mapModel.RandomSpawnLocation;
            }
        }

        public RandomSpawner(DxRectangle spawnArea, SpawnTrigger spawnTrigger) 
            : base(spawnArea, spawnTrigger)
        {
        }

        public new static RandomSpawnerBuilder Builder()
        {
            return new RandomSpawnerBuilder();
        }

        public class RandomSpawnerBuilder : Spawner.SpawnerBuilder
        {
            public override Spawner Build()
            {
                return new RandomSpawner(new DxRectangle(), spawnTrigger_);
            }
        }
    }
}
