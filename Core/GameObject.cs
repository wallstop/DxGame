using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

namespace DXGame.Core
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
    public sealed class GameObject : IIdentifiable, IEquatable<GameObject>, IProcessable
    {
        // DataMembers can't be readonly :(
        [DataMember] private List<Component> components_;
        [DataMember] private UniqueId id_ = new UniqueId();
        public IEnumerable<Component> Components => components_;

        [DataMember]
        public List<Message> CurrentMessages { get; private set; } = new List<Message>();

        [DataMember]
        public List<Message> FutureMessages { get; private set; } = new List<Message>();

        private GameObject(List<Component> components)
        {
            components_ = components;
        }

        public bool Equals(GameObject other)
        {
            return components_.Count == other.components_.Count &&
                   new HashSet<Component>(components_).SetEquals(other.components_);
        }

        public UniqueId Id => id_;
        public UpdatePriority UpdatePriority => UpdatePriority.GAME_OBJECT;

        public void Process(DxGameTime gameTime)
        {
            /* Simply swap Message buffers */
            List<Message> temp = CurrentMessages;
            CurrentMessages = FutureMessages;
            FutureMessages = temp;
            FutureMessages.Clear();
        }

        public int CompareTo(IProcessable other)
        {
            return Processable.DefaultComparer.Compare(this, other);
        }

        public void AttachComponent(Component component)
        {
            Validate.IsNotNull(component, StringUtils.GetFormattedNullOrDefaultMessage(this, component));
            Validate.IsFalse(components_.Contains(component),
                $"Cannot add a {typeof (Component)} that already exists to a {typeof (GameObject)} ");
            component.Parent = this;
            components_.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            if (components_.Contains(component))
            {
                components_.Remove(component);
                component.Parent = null;
            }
        }

        public void Remove()
        {
            DxGame.Instance.RemoveGameObject(this);
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
            return ComponentsOfType<T>().First();
        }

        public static GameObjectBuilder Builder()
        {
            return new GameObjectBuilder();
        }

        public void BroadcastMessage<T>(T message) where T : Message
        {
            FutureMessages.Add(message);
            foreach (var component in components_)
            {
                component.MessageHandler.HandleMessage(message);
            }
        }

        public class GameObjectBuilder : IBuilder<GameObject>
        {
            private List<Component> components_ = new List<Component>();

            public GameObject Build()
            {
                GameObject createdObject = new GameObject(components_);
                foreach (var component in components_)
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
                Validate.IsNotNull(enumerable, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(components)));
                foreach (var component in enumerable)
                {
                    WithComponent(component);
                }
                return this;
            }

            public GameObjectBuilder WithComponent(Component component)
            {
                Validate.IsNotNullOrDefault(component, StringUtils.GetFormattedNullOrDefaultMessage(this, component));
                Validate.IsFalse(components_.Contains(component),
                    $"Cannot create a {GetType()} with the same component {component} more than once");
                components_.Add(component);
                return this;
            }
        }
    }
}