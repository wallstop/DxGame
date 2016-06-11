using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DXGame.Core.Utils;

namespace DxCore.Core.Messaging.Entity
{
    [DataContract]
    [Serializable]
    public class EntityCreatedMessage : Message
    {
        private static readonly Predicate<object> NON_SERIALIZATION_CHECK = entity =>
        {
            var component = entity as Component;
            return component != null && !component.ShouldSerialize;
        };

        [DataMember] private Component spawnedComponent_;

        [DataMember] private GameObject spawnedGameObject_;

        public EntityCreatedMessage(Component spawnedComponent) : this(spawnedComponent, null) {}

        public EntityCreatedMessage(GameObject spawnedObject) : this(null, spawnedObject) {}

        private EntityCreatedMessage(Component spawnedComponent = null, GameObject spawnedObject = null)
        {
            Validate.IsTrue(!ReferenceEquals(spawnedComponent, null) ^ !ReferenceEquals(spawnedObject, null),
                "Expected only one of component / object to be spawned");
            spawnedComponent_ = spawnedComponent;
            spawnedGameObject_ = spawnedObject;
        }

        public bool TryGetCreatedEntity(out Component spawnedComponent)
        {
            spawnedComponent = spawnedComponent_;
            return !ReferenceEquals(spawnedComponent_, null);
        }

        public bool TryGetCreatedEntity(out GameObject spawnedGameObject)
        {
            spawnedGameObject = spawnedGameObject_;
            return !ReferenceEquals(spawnedGameObject_, null);
        }

        public override string ToString()
        {
            Component component;
            if(TryGetCreatedEntity(out component))
            {
                return component.ToString();
            }

            GameObject gameObject;
            if(TryGetCreatedEntity(out gameObject))
            {
                return gameObject.ToString();
            }

            return base.ToString();
        }
    }
}
