using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;

namespace DXGame.Core
{
/*
    pragma warning disable 649 ignores a warning for "default value is used" for the UniqueId property.
    UniqueIds are special. When they're constructed, they are gauranteed to be Unique. Therefore, we want
    a default constructed one.
*/
#pragma warning disable 649

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
        [DataMember]
        private List<Component> dxComponents_ = new List<Component>();
        [DataMember]
        private List<DrawableComponent> drawableComponents_ = new List<DrawableComponent>();
        [DataMember]
        private UniqueId id_ = new UniqueId();

        public UniqueId Id
        {
            get { return id_; }
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
            return AllComponents.OfType<T>();
        }

        public T ComponentOfType<T>() where T : Component
        {
            return ComponentsOfType<T>().First();
        }

        public List<Component> Components
        {
            get { return AllComponents.ToList(); }
        }

        private IEnumerable<Component> AllComponents
        {
            get { return dxComponents_.Union(dxComponents_).Union(drawableComponents_); }
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
            Debug.Assert(!GenericUtils.IsNullOrDefault(component), "Cannot assign a null component to a GameObject");
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
            Debug.Assert(!GenericUtils.IsNullOrDefault(components), "Cannot assign a null components to a GameObject");
            foreach (var component in components)
            {
                WithComponent(component);
            }
            return this;
        }

        // TODO: Check for prior containment
        private void AddComponent(Component component)
        {
            var drawableComponent = component as DrawableComponent;
            if (drawableComponent != null)
            {
                drawableComponent.Parent = this;
                drawableComponents_.Add(drawableComponent);
                return;
            }

            component.Parent = this;
            dxComponents_.Add(component);
        }

        public void BroadcastMessage(Message message)
        {
            foreach (var dxComponent in dxComponents_)
            {
                dxComponent.HandleMessage(message);
            }
            foreach (var drawableComponent in drawableComponents_)
            {
                drawableComponent.HandleMessage(message);
            }
        }

        public bool Equals(GameObject other)
        {
            return Id.Equals(other.Id) && new HashSet<Component>(AllComponents).SetEquals(other.AllComponents);
        }
    }
#pragma warning restore 649
}