﻿using System;
using AnimationEditorLibrary.Core.Components;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Developer;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Services
{
    public class AnimationViewerHudService : DxService
    {
        public DxVector2 SpriteSheetOffset => InternalSpriteSheetOffset;
        private UpdateableAnimationComponent AnimationPreview { get; set; }
        private static DxVector2 AnimationPreviewOffset => new DxVector2(5, 5);

        private Func<DxRectangle?> CurrentFrameBounds { get; }

        private BoundsWidget CurrentFramePreview { get; set; }
        private static DxVector2 InternalSpriteSheetOffset => new DxVector2(427, 5);
        private UpdateableSpriteComponent SpriteSheet { get; set; }

        public AnimationViewerHudService(Func<DxRectangle?> currentFrameBounds)
        {
            Validate.Hard.IsNotNull(currentFrameBounds,
                () => this.GetFormattedNullOrDefaultMessage(nameof(currentFrameBounds)));
            CurrentFrameBounds = currentFrameBounds;
        }

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
                CurrentFramePreview = new BoundsWidget(CurrentFrameBounds, Color.Crimson);
                Self.AttachComponent(CurrentFramePreview);
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
            position =
                SpatialComponent.UiBasedBuilder().WithUiOffset(InternalSpriteSheetOffset).WithoutDimensions().Build();
            spriteSheet = new UpdateableSpriteComponent(position);
        }
    }
}