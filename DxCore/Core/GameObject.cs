using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;
using NLog;

namespace DxCore.Core
{
    /**
    <summary>
        GameObjects are a wrapper that holds some bundle of components. However, each Component already knows
        what GameObject they belong to, via their Parent property. In the future, GameObjects may simply become
        a hundle UniqueId holder. Until then, they hold the knowledge of what components they contain. These, in 
        turn, are passed on to be held in structures in the main game class, where the Components will be
        properly Initialized/Updated/Drawn.

        GameObject should generally not be inherited / derived from / held references to.
    </summary>
    */

    [Serializable]
    [DataContract]
    public sealed class GameObject : IIdentifiable, IEquatable<GameObject>, IProcessable, ICreatable, IRemovable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        // DataMembers can't be readonly :(
        [DataMember] private List<Component> components_;
        [DataMember] private UniqueId id_ = new UniqueId();

        [DataMember]
        public bool Created { get; private set; }

        [DataMember]
        public bool Removed { get; private set; }

        public IEnumerable<Component> Components => components_;

        [DataMember]
        public MessageHandler MessageHandler { get; set; }

        private GameObject(List<Component> components)
        {
            components_ = new List<Component>();
            MessageHandler = new MessageHandler(Id);
            foreach(Component component in components)
            {
                AttachComponent(component);
            }
        }

        public bool Equals(GameObject other)
        {
            return components_.Count == other.components_.Count &&
                   new HashSet<Component>(components_).SetEquals(other.components_);
        }

        public UniqueId Id => id_;
        public UpdatePriority UpdatePriority => UpdatePriority.GAME_OBJECT;

        public void Process(DxGameTime gameTime) {}

        public int CompareTo(IProcessable other)
        {
            return Processable.DefaultComparer.Compare(this, other);
        }

        public void AttachComponent(Component component)
        {
            if(Removed)
            {
                return;
            }

            if(ReferenceEquals(component, null))
            {
                LOG.Info("Ignoring null component attach");
                return;
            }

            if(components_.Contains(component))
            {
                LOG.Info($"Ignoring attach of {typeof(Component)} that already exists on {this}");
                return;
            }

            component.OnDetach();
            component.Parent = this;
            component.OnAttach();
            components_.Add(component);
        }

        /**
            <summary>
                Removes all Components of the provided Type. 

                This is particularly useful for simulation purposes in pathfinding where you'd like to copy and 
                simulate the object, but not have the copy itself attempt to pathfind and simulate.
            </summary>
        */

        public void RemoveComponents<T>() where T : Component
        {
            components_.RemoveAll(component => component is T);
        }

        public void RemoveComponents(Component component)
        {
            if(components_.Contains(component))
            {
                components_.Remove(component);
                component.Parent = null;
            }
        }

        /**
        <summary>
            Given a type, iterates over all components that the game object contains and returns them as a list.

            For example, if you wanted all DrawableComponents that a GameObject has, simply:
            <code>
                GameObject myGameObject;
                List<DrawableComponent> drawables = myGameObject.ComponentsOfType<DrawableComponent>();
            </code>
        </summary>
        */

        public IEnumerable<T> ComponentsOfType<T>() where T : Component
        {
            return components_.OfType<T>();
        }

        public T ComponentOfType<T>() where T : Component
        {
            return ComponentsOfType<T>().FirstOrDefault();
        }

        public static GameObjectBuilder Builder()
        {
            return new GameObjectBuilder();
        }

        public class GameObjectBuilder : IBuilder<GameObject>
        {
            private List<Component> components_ = new List<Component>();

            public GameObject Build()
            {
                GameObject createdObject = new GameObject(components_);
                foreach(var component in components_)
                {
                    component.Parent = createdObject;
                }
                /* Reset, but don't clear, since the GameObject holds a reference to it now */
                components_ = new List<Component>();
                return createdObject;
            }

            public GameObjectBuilder WithComponents(params Component[] components)
            {
                return WithComponents(components.ToList());
            }

            public GameObjectBuilder WithComponents(IEnumerable<Component> components)
            {
                var enumerable = components as Component[] ?? components.ToArray();
                Validate.IsNotNull(enumerable, this.GetFormattedNullOrDefaultMessage(nameof(components)));
                foreach(var component in enumerable)
                {
                    WithComponent(component);
                }
                return this;
            }

            public GameObjectBuilder WithComponent(Component component)
            {
                Validate.IsNotNullOrDefault(component, this.GetFormattedNullOrDefaultMessage(component));
                Validate.IsFalse(components_.Contains(component),
                    $"Cannot create a {GetType()} with the same component {component} more than once");
                components_.Add(component);
                return this;
            }
        }

        [OnDeserialized]
        private void InitializeComponents(StreamingContext streamingContext)
        {
            foreach(Component component in components_)
            {
                component.DeSerialize();
            }
        }

        public void Remove()
        {
            if(!Removed)
            {
                foreach(Component component in components_)
                {
                    EntityRemovedMessage componentRemoved = new EntityRemovedMessage(component);
                    componentRemoved.Emit();
                }

                EntityRemovedMessage gameObjectRemoved = new EntityRemovedMessage(this);
                gameObjectRemoved.Emit();
                MessageHandler.Deregister();
            }
            Removed = true;
        }

        public void Create()
        {
            if(!Created)
            {
                EntityCreatedMessage entityCreated = new EntityCreatedMessage(this);
                entityCreated.Emit();
            }
            Created = true;
        }
    }
}