using AnimationEditorLibrary.Core.Messaging;
using DxCore;
using DxCore.Core.Animation;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Distance;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Components
{
    public class UpdateableAnimationComponent : DrawableComponent
    {
        private Animation Animation { get; set; }

        private Direction Orientation { get; set; } = Direction.East;

        private IPositional Position { get; }

        public UpdateableAnimationComponent(IPositional position)
        {
            Validate.Hard.IsNotNull(position);
            Position = position;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Animation?.Draw(spriteBatch, gameTime, Position.WorldCoordinates, Orientation);
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<AnimationChangedMessage>(HandleAnimationChanged);
            RegisterMessageHandler<OrientationChangedMessage>(HandleOrientationChanged);
            base.OnAttach();
        }

        private void HandleAnimationChanged(AnimationChangedMessage newAnimation)
        {
            if(0 < newAnimation.Descriptor.FrameCount)
            {
                Animation = new Animation(newAnimation.Descriptor);
                Animation.LoadContent(DxGame.Instance.Content);
            }
            else
            {
                Animation = null;
            }
        }

        private void HandleOrientationChanged(OrientationChangedMessage orientationChanged)
        {
            Orientation = orientationChanged.Orientation;
        }
    }
}