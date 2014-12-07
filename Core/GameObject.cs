using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        GameObject should gemerally not be inherited / derived from / held references to.
    </summary>
    */

    public class GameObject
    {
        private readonly List<Component> components_ = new List<Component>();
        private readonly UniqueId id_ = new UniqueId();

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
        public List<T> ComponentsOfType<T>() where T : Component
        {
            return components_.OfType<T>().ToList();
        }

        /**
        <summary>
            Given a component, properly determines if it is a Drawable / Initializable / Updateable, adds it
            to the GameObject, and returns a reference to the updated GameObject.

            The following code will produce a GameObject with component1 and component2 attached.
            <code>
                GameObject object = new GameObject().AttachComponent(component1).AttachComponent(component2);
            </code>
        </summary>
        */

        protected GameObject AttachComponent(Component component)
        {
            Debug.Assert(component != null, "Cannot assign a null component to a GameObject");
            components_.Add(component);
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

        public GameObject AttachComponents(params Component[] components)
        {
            Debug.Assert(components != null, "Cannot assign a null components to a GameObject");
            components_.AddRange(components);
            return this;
        }
    }
#pragma warning restore 649
}