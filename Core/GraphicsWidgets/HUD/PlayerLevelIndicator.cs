using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Components.Basic;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    [DataContract]
    [Serializable]
    public class PlayerLevelIndicator : DrawableComponent
    {
        [DataMember]
        public DxVector2 ScreenOffset { get; }

        public PlayerLevelIndicator(DxVector2 screenOffset)
        {
            Validate.IsTrue(screenOffset.X >= 0, $"Cannot create a {typeof(PlayerLevelIndicator)} with an x offset of {screenOffset.X}");
            Validate.IsTrue(screenOffset.X >= 0, $"Cannot create a {typeof(PlayerLevelIndicator)} with an x offset of {screenOffset.X}");
            ScreenOffset = screenOffset;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Player activePlayer = DxGame.Instance.Model<PlayerModel>().ActivePlayer;
            LevelComponent levelComponent = activePlayer.Level;

            double percentThroughLevel = levelComponent.Progress;
            string currentLevelAsText = levelComponent.Level.ToString();

            Vector2 drawLocation = DxGame.Instance.OffsetFromScreen(ScreenOffset);

            // TODO

            throw new NotImplementedException();
        }
    }
}
