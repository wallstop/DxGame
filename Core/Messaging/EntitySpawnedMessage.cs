﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging
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
        public UniqueId SpawnerId { get; }

        public Optional<WeakReference<Component>> SpawnedComponent { get; }

        public Optional<WeakReference<GameObject>> SpawnedObject { get; }

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

            SpawnedComponent = Optional<WeakReference<Component>>.Of(new WeakReference<Component>(spawnedComponent));
            SpawnedObject = Optional<WeakReference<GameObject>>.Of(new WeakReference<GameObject>(spawnedObject));
        }
    }
}