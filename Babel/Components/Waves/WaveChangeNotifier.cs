using System;
using System.Runtime.Serialization;
using Babel.Messaging;
using DxCore;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.GraphicsWidgets;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Components.Waves
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
        private SpatialComponent Position { get; set; }

        [DataMember]
        private DxVector2 Offset { get; set; }

        private bool Active => DxGame.Instance.CurrentUpdateTime.TotalGameTime < lastWaveNotification_ + DISPLAY_TIME;

        public WaveChangeNotifier()
        {
            Position =
                SpatialComponent.UiTrackingBasedBuilder().WithUiOffsetProvider(GetOffset).WithoutDimensions().Build();
        }

        private DxVector2 GetOffset()
        {
            return Offset;
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
            Offset = drawTarget;
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