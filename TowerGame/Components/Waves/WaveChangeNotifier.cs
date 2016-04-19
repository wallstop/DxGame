using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using DXGame.TowerGame.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.TowerGame.Components.Waves
{
    /**
        <summary>
            Handles listening for NewWaveMessages, notifying the player that this has happened (WOW)
        </summary>
    */

    [DataContract]
    [Serializable]
    public class WaveChangeNotifier : DrawableComponent
    {
        private static readonly TimeSpan DISPLAY_TIME = TimeSpan.FromSeconds(2.5);

        private static readonly string WAVE_TEXT_BASE = "Wave ";

        [DataMember] private TimeSpan lastWaveNotification_;

        [NonSerialized] [IgnoreDataMember] private SpriteFont spriteFont_;

        [DataMember]
        private TextComponent WaveText { get; set; }

        [DataMember]
        private PositionalComponent Position { get; }

        private bool Active => DxGame.Instance.CurrentTime.TotalGameTime < lastWaveNotification_ + DISPLAY_TIME;

        public WaveChangeNotifier()
        {
            Position = PositionalComponent.Builder().Build();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<NewWaveMessage>(HandleWaveNotification);
            base.OnAttach();
        }

        private void HandleWaveNotification(NewWaveMessage newWaveMessage)
        {
            lastWaveNotification_ = newWaveMessage.TimeStamp;
            if(ReferenceEquals(WaveText, null))
            {
                // Not initialized yet, ignore
                // TODO: Come up with better lifetime management :(
                return;
            }

            WaveText.Text = WAVE_TEXT_BASE + newWaveMessage.WaveNumber;

            Vector2 textSize = spriteFont_.MeasureString(WaveText.Text);

            Rectangle baseScreenSize = DxGame.Instance.Screen;
            Point center = baseScreenSize.Center;
            DxVector2 drawTarget = new DxVector2(center.X - textSize.X / 2, center.Y - textSize.Y / 2);
            Vector2 offset = DxGame.Instance.OffsetFromScreen(drawTarget);
            Position.Position = new DxVector2(offset);
        }

        public override void LoadContent()
        {
            // TODO: Make a FontCache
            spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles");
            WaveText = new TextComponent(Position, spriteFont_, "Fonts/Pericles");
        }

        private static Color DetermineColorFade(TimeSpan currentTime, TimeSpan max)
        {
            double current = Math.Max(0, currentTime.TotalMilliseconds);
            current = Math.Min(current, max.TotalMilliseconds);
            double end = max.TotalMilliseconds;
            double scalar = SpringFunctions.Linear(1, 0, current, end);
            Color transparentColor = ColorFactory.Transparency((float) scalar, Color.Purple);
            return transparentColor;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(!Active)
            {
                return;
            }
            Color fadedColor = DetermineColorFade(gameTime.TotalGameTime - lastWaveNotification_, DISPLAY_TIME);
            WaveText.DxColor = new DxColor(fadedColor);
            WaveText.Draw(spriteBatch, gameTime);
        }

        public override void Remove()
        {
            Position.Remove();
            WaveText.Remove();
            base.Remove();
        }
    }
}