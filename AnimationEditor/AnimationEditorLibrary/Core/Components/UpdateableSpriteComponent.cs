using System;
using System.IO;
using AnimationEditorLibrary.Core.Messaging;
using DxCore;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Components
{
    /**
        <summary>
            A simple, non-animated sprite drawing component that updates itself in response to animation changed messages
        </summary>
    */

    public class UpdateableSpriteComponent : DrawableComponent
    {
        private DxVector2 DrawOffset { get; set; }
        private string PreviousAssetPath { get; set; }
        private float Scale { get; set; } = 1.0f;
        private ISpatial Spatial { get; }
        private Texture2D Sprite { get; set; }

        public UpdateableSpriteComponent(ISpatial spatial)
        {
            Validate.Hard.IsNotNull(spatial);
            Spatial = spatial;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(Validate.Check.IsNull(Sprite))
            {
                return;
            }

            Rectangle target = Sprite.Bounds;
            target.Width = (int) (target.Width * Scale);
            target.Height = (int) (target.Height * Scale);
            target.X = (int) Math.Round(Spatial.WorldCoordinates.X + DrawOffset.X);
            target.Y = (int) Math.Round(Spatial.WorldCoordinates.Y + DrawOffset.Y);
            spriteBatch.Draw(Sprite, target, Color.White);
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<AnimationChangedMessage>(HandleAnimationChanged);
            RegisterMessageHandler<SpriteSheetOffsetChangedMessage>(HandleDrawOffsetChanged);
        }

        private void HandleAnimationChanged(AnimationChangedMessage animationChanged)
        {
            Scale = animationChanged.Descriptor.Scale;
            if(Objects.Equals(PreviousAssetPath, animationChanged.AssetPath))
            {
                return;
            }
            PreviousAssetPath = animationChanged.AssetPath;

            using(Stream fileStream = File.Open(animationChanged.AssetPath, FileMode.Open))
            {
                Sprite = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, fileStream);
            }
        }

        private void HandleDrawOffsetChanged(SpriteSheetOffsetChangedMessage spriteSheetOffsetChanged)
        {
            DrawOffset = spriteSheetOffsetChanged.LatesOffset;
        }
    }
}