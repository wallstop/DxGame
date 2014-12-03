using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Basic
{
    /**
    <summary>
        DrawableComponent forms the base of all Components that wish to be rendered at some point in time.
        Each derived class must implement their own LoadContent and Draw methods, which will be called
        in the main LoadContent and Draw methods. 

        Components using this class will typically be sprites, with or without animations. Particle effects, etc.
            
        <see cref=SimpleSpriteComponent />

        For other base components, 
        <see cref=InitializbleComponent />
        <see cref=UpdateableComponent />
    </summary>
    */

    public abstract class DrawableComponent : Component
    {
        protected DrawableComponent(GameObject parent = null)
            : base(parent)
        {
        }

        /**
        <summary>
            Loads any relevant content from either the Content pipeline (http://msdn.microsoft.com/en-us/library/bb447745.aspx)
            or generated methods. This will typically involve loading a sprite sheet by name.

            See http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.loadcontent.aspx for information about 
            Game.LoadContent, which will invoke this method.
        </summary>
        */
        public abstract bool LoadContent(ContentManager contentManager);

        /**
        <summary>
            Draws the relevant textures to a spriteBatch. This method gets called once per DrawableComponent rendered (in the scene)
            per frame. Logic that depends on staying in-sync with Game Time should *NOT* be put here. The correct place for
            updateable logic is in an UpdateableComponent.

            See http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.game.draw.aspx for information about
            Game.Draw, which will invoke this method.
        </summary>
        */
        public abstract bool Draw(SpriteBatch spriteBatch);
    }
}