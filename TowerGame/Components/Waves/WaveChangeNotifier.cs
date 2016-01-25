using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Triggers;
using DXGame.Core.Components.Basic;
using DXGame.Core.Events;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Messaging;
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

        [DataMember] private SpriteFont spriteFont_;

        [DataMember]
        private TextComponent WaveText { get; set; }

        [DataMember]
        private PositionalComponent Position { get; }

        [DataMember]
        private EventObserver WaveMessageObserver { get; }

        private bool Active => DxGame.Instance.CurrentTime.TotalGameTime < (lastWaveNotification_ + DISPLAY_TIME);

        public WaveChangeNotifier()
        {
            WaveMessageObserver =
                EventObserver.EventObserverBuilder()
                    .WithAcceptance(CheckIsWaveNotification)
                    .WithAction(HandleWaveNotificationEvent)
                    .Build();
            MessageHandler.RegisterMessageHandler<NewWaveMessage>(HandleWaveNotification);
            Position = PositionalComponent.Builder().Build();
        }

        private bool CheckIsWaveNotification(Event gameEvent)
        {
            Message eventMessage = gameEvent.Message;
            NewWaveMessage maybeNewWaveMessage = eventMessage as NewWaveMessage;
            return !ReferenceEquals(maybeNewWaveMessage, null);
        }

        private void HandleWaveNotificationEvent(Event gameEvent)
        {
            Message eventMessage = gameEvent.Message;
            NewWaveMessage newWaveMessage = eventMessage as NewWaveMessage;
            HandleWaveNotification(newWaveMessage);
        }

        private void HandleWaveNotification(NewWaveMessage newWaveMessage)
        {
            lastWaveNotification_ = newWaveMessage.TimeStamp;
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
            WaveText = new TextComponent(Position, spriteFont_);
        }

        private static Color DetermineColorFade(TimeSpan currentTime, TimeSpan max)
        {
            double current = currentTime.TotalMilliseconds;
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
            Color fadedColor = DetermineColorFade(gameTime.TotalGameTime - lastWaveNotification_,
                DISPLAY_TIME);
            WaveText.Color = fadedColor;
            WaveText.Draw(spriteBatch, gameTime);
        }

        public override void Dispose()
        {
            Position.Dispose();
            WaveMessageObserver.Dispose();
            WaveText.Dispose();
            base.Dispose();
        }
    }
}