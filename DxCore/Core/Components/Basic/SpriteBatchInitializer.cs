using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DXGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Basic
{
    [Serializable]
    [DataContract]
    public class SpriteBatchInitializer : DrawableComponent
    {
        public SpriteBatchInitializer()
        {
            DrawPriority = DrawPriority.INIT_SPRITEBATCH;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // TODO: Change this ugly color
            DxGame.Instance.GraphicsDevice.Clear(Color.DarkGray);

            DxRectangle screen = DxGame.Instance.ScreenRegion;

            Matrix cameraShift = Matrix.CreateTranslation(screen.X, screen.Y, 0);

            /*
                http://gamedev.stackexchange.com/questions/19761/in-xna-3-1-is-it-possible-to-disable-texture-filtering
                http://gamedev.stackexchange.com/questions/6820/how-do-i-disable-texture-filtering-for-sprite-scaling-in-xna-4-0
                http://stackoverflow.com/questions/8130149/how-to-set-xnas-texturefilter-to-point

                We don't want *ANY* smoothing happening with any of our sprites. This causes "nice" antialiasing, which for us, is shit.
                We're aiming for a "pixel art" kind of game, not a "shitty psuedo-pixel art" kind of game.

                Assigning SamplerState to PointClamp mode here allows us to preserve pixel-perfect clarity while scaling images both up
                and down, giving us that sweet pixel look that we're aiming for.
            */
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraShift);
        }
    }
}