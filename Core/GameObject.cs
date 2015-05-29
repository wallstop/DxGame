using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

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
    public class GameObject : IIdentifiable, IEquatable<GameObject>
    {
        // DataMembers can't be readonly :(
        [DataMember] private List<Component> components_ = new List<Component>();
        [DataMember] private UniqueId id_ = new UniqueId();
        public IEnumerable<Component> Components => components_;

        public bool Equals(GameObject other)
        {
            return components_.Count == other.components_.Count &&
                   new HashSet<Component>(components_).SetEquals(other.components_);
        }

        public UniqueId Id => id_;
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

        /**
        <summary>
            Given a component, properly determines if it is a Drawable / Initializable / Updateable, adds it
            to the GameObject, and returns a reference to the updated GameObject.

            The following code will produce a GameObject with component1 and component2 attached.
            <code>
                GameObject object = new GameObject().WithComponent(component1).WithComponent(component2);
            </code>
        </summary>
        */

        public GameObject WithComponent(Component component)
        {
            Validate.IsNotNullOrDefault(component, $"Cannot add a null {typeof (Component)} to {GetType()}");
            AddComponent(component);
            return this;
        }

        /**
        <summary>
            Given some number of components, properly sorts them out into 
            Drawables / Initializables / Updateables, adds them to the 
            GameObject, and returns a reference to the updated GameObject.
            
            The following code will produce a GameObject with component1, component2, and component3 
            attached.
            <code>
                GameObject object = new GameObject().AttachComponents(component1, component2, component3);
            </code>
        </summary>
        */

        public GameObject WithComponents(params Component[] components)
        {
            Validate.IsNotNullOrDefault(components,
                $"Cannot add a null/empty {typeof (Component)} collection to {GetType()}");
            foreach (var component in components)
            {
                WithComponent(component);
            }
            return this;
        }

        // TODO: Check for prior containment
        private void AddComponent(Component component)
        {
            Validate.IsNotNull(component, $"Cannot attach a null {nameof(Component)} to a {GetType()}");
            Validate.IsFalse(components_.Contains(component));
            component.Parent = this;
            components_.Add(component);
        }

        public void BroadcastMessage(Message message)
        {
            foreach (var component in components_)
            {
                component.HandleMessage(message);
            }
        }
    }
}