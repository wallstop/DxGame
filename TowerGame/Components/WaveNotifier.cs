using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Main;
using DXGame.TowerGame.Messaging;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.TowerGame.Components
{
    [Serializable]
    public class WaveNotifier : DrawableComponent
    {
        private static readonly TimeSpan DISPLAY_TIME = TimeSpan.FromSeconds(2.5);

        private SpriteFont spriteFont_;

        public WaveNotifier()
        {
            MessageHandler.RegisterMessageHandler<NewWaveMessage>(HandleWaveNotification);
        }

        private void HandleWaveNotification(NewWaveMessage newWaveMessage)
        {
            // TODO TODO TODO 
        }

        public override void LoadContent()
        {
            spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles");
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
