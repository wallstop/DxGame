using System;
using System.Runtime.Serialization;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Entities
{
    /**

        <summary>
            A spawner that repositions the spawn location of every entity to somewhere (hopefully) reachable on the map
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class RandomSpawner : Spawner
    {
        protected override DxRectangle SpawnArea
        {
            get
            {
                MapModel mapModel = DxGame.Instance.Model<MapModel>();
                return mapModel.RandomSpawnLocation;
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
