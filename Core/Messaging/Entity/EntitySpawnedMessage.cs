using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Entity
{
    /**
        <summary>
            Notification that an entity has Spawned
        </summary>
    */

    [Serializable]
    [DataContract]
    public class EntitySpawnedMessage : Message
    {
        [DataMember]
        public UniqueId SpawnerId { get; private set; }

        [DataMember] private Component spawnedComponent_;

        [DataMember] private GameObject spawnedGameObject_;
        
        public EntitySpawnedMessage(UniqueId spawnerId, Component spawnedComponent)
            : this(spawnerId, spawnedComponent, null) {}

        public EntitySpawnedMessage(UniqueId spawnerId, GameObject spawnedObject) : this(spawnerId, null, spawnedObject) {}

        private EntitySpawnedMessage(UniqueId spawnerId, Component spawnedComponent = null,
            GameObject spawnedObject = null)
        {
            Validate.IsNotNull(spawnerId, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(spawnerId)));
            Validate.IsTrue(!ReferenceEquals(spawnedComponent, null) ^ !ReferenceEquals(spawnedObject, null),
                "Expected only one of component / object to be spawned");
            SpawnerId = spawnerId;
            spawnedComponent_ = spawnedComponent;
            spawnedGameObject_ = spawnedObject;
        }

        public bool TryGetSpawnedEntity(out Component spawnedComponent)
        {
            spawnedComponent = spawnedComponent_;
            return !ReferenceEquals(spawnedComponent_, null);
        }

        public bool TryGetSpawnedEntity(out GameObject spawnedGameObject)
        {
            spawnedGameObject = spawnedGameObject_;
            return !ReferenceEquals(spawnedGameObject_, null);
        }

        public override string ToString()
        {
            Component component;
            if(TryGetSpawnedEntity(out component))
            {
                return component.ToString();
            }

            GameObject gameObject;
            if(TryGetSpawnedEntity(out gameObject))
            {
                return gameObject.ToString();
            }

            return base.ToString();
        }
    }
}