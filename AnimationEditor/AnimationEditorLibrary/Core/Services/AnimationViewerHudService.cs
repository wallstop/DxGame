using AnimationEditorLibrary.Core.Components;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Developer;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Services
{
    public class AnimationViewerHudService : DxService
    {
        private UpdateableAnimationComponent AnimationPreview { get; set; }
        private static DxVector2 AnimationPreviewOffset => new DxVector2(5, 5);

        private BoundingBoxWidget CurrentFramePreview { get; set; }
        private UpdateableSpriteComponent SpriteSheet { get; set; }
        private static DxVector2 SpriteSheetOffset => new DxVector2(427, 5);

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(AnimationPreview))
            {
                SpatialComponent position;
                UpdateableAnimationComponent animationPreview;
                GenerateAnimationPreview(out position, out animationPreview);
                AnimationPreview = animationPreview;
                Self.AttachComponent(AnimationPreview);
                Self.AttachComponent(position);
            }
            if(Validate.Check.IsNull(SpriteSheet))
            {
                SpatialComponent position;
                UpdateableSpriteComponent spriteSheet;
                GenerateSpriteSheetPreview(out position, out spriteSheet);
                SpriteSheet = spriteSheet;
                Self.AttachComponent(position);
                Self.AttachComponent(SpriteSheet);
            }
            if(Validate.Check.IsNull(CurrentFramePreview))
            {
                //CurrentFramePreview =
            }
        }

        private static void GenerateAnimationPreview(out SpatialComponent position,
            out UpdateableAnimationComponent animationPreview)
        {
            position =
                SpatialComponent.UiBasedBuilder().WithUiOffset(AnimationPreviewOffset).WithoutDimensions().Build();
            animationPreview = new UpdateableAnimationComponent(position);
        }

        private static void GenerateSpriteSheetPreview(out SpatialComponent position,
            out UpdateableSpriteComponent spriteSheet)
        {
            position = SpatialComponent.UiBasedBuilder().WithUiOffset(SpriteSheetOffset).WithoutDimensions().Build();
            spriteSheet = new UpdateableSpriteComponent(position);
        }
    }
}