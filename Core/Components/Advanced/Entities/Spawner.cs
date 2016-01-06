using System;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Entities
{
    /*
        Simple function that, given a GameTime, returns true if a GameObject should be spawned there
        along with the GameObject that should be spawned. If the first element in the tuple is false,
        the GameObject reference is undefined.
    */

    public delegate Tuple<bool, GameObject> SpawnTrigger(DxGameTime gameTime);

    [Serializable]
    public class Spawner : Component
    {
        protected DxRectangle spawnArea_;
        protected SpawnTrigger SpawnTrigger { get; }

        protected virtual DxRectangle SpawnArea => spawnArea_;

        protected Spawner(DxRectangle spawnArea, SpawnTrigger spawnTrigger)
        {
            Validate.IsNotNull(spawnTrigger);
            SpawnTrigger = spawnTrigger;
            spawnArea_ = spawnArea;
        }

        public static SpawnTrigger ImmediateSpawnTriggerFor(GameObject gameObject)
        {
            SimpleImmediateSpawner immediateSpawner = new SimpleImmediateSpawner(gameObject);
            return immediateSpawner.SpawnTrigger;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var spawnCheck = SpawnTrigger(gameTime);
            if(spawnCheck.Item1)
            {
                GameObject spawnedObject = spawnCheck.Item2;
                PositionalComponent position = spawnedObject.Components.OfType<PositionalComponent>().FirstOrDefault();
                if(!ReferenceEquals(position, null))
                {
                    position.Position = RandomPositionInSpawnArea();
                }
                DxGame.Instance.AddAndInitializeGameObject(spawnedObject);
                EntitySpawnedMessage spawnMessage = new EntitySpawnedMessage(Id, spawnedObject);
                DxGame.Instance.BroadcastMessage(spawnMessage);
            }
            base.Update(gameTime);
        }

        private DxVector2 RandomPositionInSpawnArea()
        {
            var spawnArea = SpawnArea;
            var minX = spawnArea.X;
            var maxX = spawnArea.X + spawnArea.Width;
            var minY = spawnArea.Y;
            var maxY = spawnArea.Y + spawnArea.Height;

            var xCoordinate = ThreadLocalRandom.Current.NextFloat(minX, maxX);
            var yCoordinate = ThreadLocalRandom.Current.NextFloat(minY, maxY);
            return new DxVector2(xCoordinate, yCoordinate);
        }

        public static SpawnerBuilder Builder()
        {
            return new SpawnerBuilder();
        }

        public class SpawnerBuilder : IBuilder<Spawner>
        {
            protected DxRectangle spawnArea_;
            protected SpawnTrigger spawnTrigger_;

            public virtual Spawner Build()
            {
                Validate.IsNotNullOrDefault(spawnTrigger_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "SpawnTrigger"));
                return new Spawner(spawnArea_, spawnTrigger_);
            }

            public SpawnerBuilder WithSpawnArea(DxRectangle spawnArea)
            {
                spawnArea_ = spawnArea;
                return this;
            }

            public SpawnerBuilder WithSpawnTrigger(SpawnTrigger trigger)
            {
                spawnTrigger_ = trigger;
                return this;
            }
        }
    }

    /**
        <summary>
            Simple helper for determing if an entity has already spawned
        </summary>
    */

    [DataContract]
    [Serializable]
    internal class SimpleImmediateSpawner
    {
        private readonly GameObject objectToSpawn_;
        private bool alreadySpawned_;

        public SimpleImmediateSpawner(GameObject objectToSpawn)
        {
            Validate.IsNotNullOrDefault(objectToSpawn,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(objectToSpawn)));
            objectToSpawn_ = objectToSpawn;
            alreadySpawned_ = false;
        }

        public Tuple<bool, GameObject> SpawnTrigger(DxGameTime gameTime)
        {
            if(alreadySpawned_)
            {
                return Tuple.Create<bool, GameObject>(false, null);
            }
            alreadySpawned_ = true;
            return Tuple.Create(true, objectToSpawn_);
        }
    }
}