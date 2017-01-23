using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.GraphicsWidgets.HUD
{
    [DataContract]
    [Serializable]
    public class PlayerLevelIndicator : DrawableComponent
    {
        [DataMember]
        public DxVector2 ScreenOffset { get; }

        public PlayerLevelIndicator(DxVector2 screenOffset)
        {
            Validate.Hard.IsTrue(screenOffset.X >= 0,
                () => $"Cannot create a {typeof(PlayerLevelIndicator)} with an x offset of {screenOffset.X}");
            Validate.Hard.IsTrue(screenOffset.X >= 0,
                () => $"Cannot create a {typeof(PlayerLevelIndicator)} with an x offset of {screenOffset.X}");
            ScreenOffset = screenOffset;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Player activePlayer = DxGame.Instance.Service<PlayerService>().ActivePlayer;
            EntityLevelComponent entityLevelComponent = activePlayer.EntityLevel;

            double percentThroughLevel = entityLevelComponent.Progress;
            string currentLevelAsText = entityLevelComponent.Level.ToString();

            Vector2 drawLocation = DxGame.Instance.OffsetFromScreen(ScreenOffset);

            // TODO

            throw new NotImplementedException();
        }
    }
}