using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Components.Basic;

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
        private readonly List<DrawableComponent> drawables_ = new List<DrawableComponent>();
        private readonly UniqueId id_ = new UniqueId();
        private readonly List<UpdateableComponent> updateables_ = new List<UpdateableComponent>();
        private readonly List<InitializableComponent> initializables_ = new List<InitializableComponent>();

        public UniqueId Id
        {
            get { return id_; }
        }

        public List<DrawableComponent> Drawables
        {
            get { return drawables_; }
        }

        public List<UpdateableComponent> Updateables
        {
            get { return updateables_; }
        }

        public List<InitializableComponent> Initializables
        {
            get { return initializables_; }
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
            DetermineAndAssignComponentType(component);
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
            foreach (Component component in components)
            {
                DetermineAndAssignComponentType(component);
            }
            return this;
        }

        /*
            Determines the type of a Component that has been attached. If it is
            an Initializable, Drawable, or Updateable, adds it to the appropriate member list.
        */

        private void DetermineAndAssignComponentType(Component component)
        {
            Debug.Assert(component != null, "Cannot assign a null component to a GameObject");
            /*
                TODO: Come up with a better (re: remotely good) way of properly assigning / distributing
                comopnents into their categories. Maybe a virtual method on each component like .ComponentType
                that the base classes would override? Maybe a .IsDrawable? Whatever it is, this really sucks, and
                should be fixed.
            */
            var drawable = component as DrawableComponent;
            if (drawable != null)
            {
                drawables_.Add(drawable);
                return;
            }

            var updateable = component as UpdateableComponent;
            if (updateable != null)
            {
                updateables_.Add(updateable);
                return;
            }

            var initializable = component as InitializableComponent;
            if (initializable != null)
            {
                initializables_.Add(initializable);
            }
        }
    }
#pragma warning restore 649
}