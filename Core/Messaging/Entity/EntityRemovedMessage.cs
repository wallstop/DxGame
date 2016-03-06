using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DXGame.Core.Messaging.Entity
{
    [DataContract]
    [Serializable]
    public class EntityRemovedMessage : Message
    {
        [DataMember] private Component removedComponent_;

        [DataMember] private GameObject removedGameObject_;

        public EntityRemovedMessage(Component removedComponent) : this(removedComponent, null) {}

        public EntityRemovedMessage(GameObject removedGameObject) : this(null, removedGameObject) {}

        private EntityRemovedMessage(Component removedComponent = null, GameObject removedGameObject = null)
        {
            Validate.IsTrue(!ReferenceEquals(removedComponent, null) ^ !ReferenceEquals(removedGameObject, null),
                "Expected only one of component / object to be spawned");

            removedComponent_ = removedComponent;
            removedGameObject_ = removedGameObject;
        }

        public bool TryGetRemovedEntity(out Component removedComponent)
        {
            removedComponent = removedComponent_;
            return !ReferenceEquals(removedComponent_, null);
        }

        public bool TryGetRemovedEntity(out GameObject removedGameObject)
        {
            removedGameObject = removedGameObject_;
            return !ReferenceEquals(removedGameObject, null);
        }

        public override bool Global => true;
    }
}
