namespace DXGame.Core.Components.Basic
{
    /**
    <summary>
        InitializableComponent forms the base class of all Components that require an initialization step. This typically
        will involve database connections, TCP/IP connections, possible seeding from some state (random numbers?), and 
        other things of the sort.

        Currently, no components use this class.

        For other base components, 
        <see cref=DrawableComponent />
        <see cref=UpdatableComponent />
    */

    public abstract class InitializableComponent : Component
    {
        protected InitializableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        /**
        <summary>
            Initializes the component to some state. 

            See http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.initialize.aspx for
            information about Game.Initialize, which will invoke this method.
        </summary>
        */
        public abstract bool Initialize();
    }
}