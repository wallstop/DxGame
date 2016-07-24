using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Services;

namespace DxCore.Core.Components.Advanced.Entities
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
        protected override DxRectangle SpawnArea
        {
            get
            {
                MapService mapService = DxGame.Instance.Service<MapService>();
                return mapService.RandomSpawnLocation;
            }
        }

        public RandomSpawner(DxRectangle spawnArea, SpawnTrigger spawnTrigger) : base(spawnArea, spawnTrigger) {}

        public new static RandomSpawnerBuilder Builder()
        {
            return new RandomSpawnerBuilder();
        }

        public class RandomSpawnerBuilder : SpawnerBuilder
        {
            public override Spawner Build()
            {
                return new RandomSpawner(new DxRectangle(), spawnTrigger_);
            }
        }
    }
}
