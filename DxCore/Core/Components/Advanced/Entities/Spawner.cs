using System;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Entities
{
    /*
        Simple function that, given a GameTime, returns true if a GameObject should be spawned there
        along with the GameObject that should be spawned. If the first element in the tuple is false,
        the GameObject reference is undefined.
    */

    // TODO: Refactor to use be delegate bool SpawnTrigger(out GameObject gameObject, DxGameTime gameTime);
    public delegate Tuple<bool, GameObject> SpawnTrigger(DxGameTime gameTime);

    [DataContract]
    [Serializable]
    public class Spawner : Component
    {
        [DataMember] protected DxRectangle spawnArea_;

        [DataMember]
        protected SpawnTrigger SpawnTrigger { get; }

        protected virtual DxRectangle SpawnArea => spawnArea_;

        protected Spawner(DxRectangle spawnArea, SpawnTrigger spawnTrigger)
        {
            Validate.Hard.IsNotNull(spawnTrigger);
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
                PhysicsComponent position = spawnedObject.ComponentOfType<PhysicsComponent>();
                if(!ReferenceEquals(position, null))
                {
                    // TODO: Fix direct position access
                    position.Position = RandomPositionInSpawnArea();
                }

                spawnedObject.Create();
                EntitySpawnedMessage spawnMessage = new EntitySpawnedMessage(Id, spawnedObject);
                spawnMessage.Emit();
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
                Validate.Hard.IsNotNullOrDefault(spawnTrigger_,
                    this.GetFormattedNullOrDefaultMessage(nameof(Entities.SpawnTrigger)));
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
        [DataMember] private readonly GameObject objectToSpawn_;
        [DataMember] private bool alreadySpawned_;

        public SimpleImmediateSpawner(GameObject objectToSpawn)
        {
            Validate.Hard.IsNotNullOrDefault(objectToSpawn, this.GetFormattedNullOrDefaultMessage(nameof(objectToSpawn)));
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