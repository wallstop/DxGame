namespace DXGame.Core
{
    /**
    <summary>
        This class forms the base class for all Components. Components are a methodology for 
        decoupling entity-specific behavior and logic from their implementation.

        For information about the Component pattern, see: http://gameprogrammingpatterns.com/component.html

        Components fall into four main categories: Drawable, Initializable, Updateable, and none of the above.
        <see ref=DrawableComponent />
        <see ref=InitializableComponent />
        <see ref=UpdateableComponent />

        Components will make up the core of all "Gameplay elements". The map is made of components. Input is
        handled via components. Animations are handled via components. AI is handled via components. Everything
        is components!

        Creating a new Component requires a bit of design. The main question that should be addressed is the
        Is-A versus Has-A relationship. Am I an UpdateableComponent or should I have one? For examples on this,
        <see ref=Components/Advanced />

        To create a Component, simply derive from Component (or one of the Basic/Advanced ones)
        <code>
            public class NameableComponent : Component
            {
                private string name_;
            
                public string Name
                { 
                    get { return name_; }
                }

                public NameableComponent(string name, GameObject parent = null)
                    : base(parent)
                {
                    name_ = name;
                }   
            }
        </code>
    </summary>            
    */
    public abstract class Component
    {
        /**
            Note: This id_ field is the UniqueId of the Component, *NOT* of the GameObject. 
            This is a very important distinction.
        */
        protected readonly UniqueId id_;

        public UniqueId Id
        {
            get { return id_; }
        }

        /**
        <summary>
            The "owning" GameObject of this component. 

            Note: This field does not have to contain a valid GameObject. A (poorly constructed) Component 
            can easily act on a GameObject. Generally, a Component's Parent property should point to a valid
            and correct owning GameObject, but this shouldn't be relied on.
        </summary>
        */
        public GameObject Parent { get; set; }

        protected Component(GameObject parent = null)
        {
            Parent = parent;
        }
    }
}