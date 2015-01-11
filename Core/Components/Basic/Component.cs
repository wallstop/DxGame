using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Basic
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

                public NameableComponent(Game game)
                    : base(game)
                {
                }
   
                public NameableComponent WithName(string name)
                {
                    Debug.Assert(!GenericUtils.IsNullOrDefault(name), "NameableComponent cannot be initialized with a null name");
                    name_ = name;
                }
            }
        </code>
    </summary>            
    */

    public abstract class Component : GameComponent
    {
        /**
            Note: This id_ field is the UniqueId of the Component, *NOT* of the GameObject. 
            This is a very important distinction.
        */
        protected readonly UniqueId id_ = new UniqueId();

        public UniqueId Id
        {
            get { return id_; }
        }

        public DxGame DxGame
        {
            get { return (DxGame) Game; }
        }

        protected Component(DxGame game)
            : base(game)
        {
            UpdatePriority = UpdatePriority.NORMAL;
        }

        protected UpdatePriority UpdatePriority
        {
            set { UpdateOrder = (int) value; }
            get { return (UpdatePriority) UpdateOrder; }
        }
    }

    public enum UpdatePriority
    {
        HIGHEST = -1,
        PHYSICS = HIGHEST,
        HIGH = 1,
        NORMAL = 5,
        WORLD_GRAVITY = NORMAL,
        LOW = 10
    }
}