using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DXGame.Core.Animation
{
    public enum StandardAnimationType
    {
        Idle,
        WalkingLeft,
        WalkingRight,
        JumpLeft,
        JumpRight
    }

    public class AnimationFactory
    {
        private static readonly string CONTENT_PATH = "Content";

        private static readonly Lazy<AnimationFactory> SINGLETON =
            new Lazy<AnimationFactory>(() => new AnimationFactory());

        private static readonly ReadOnlyDictionary<StandardAnimationType, string> STANDARD_FILE_NAMES =
            new ReadOnlyDictionary<StandardAnimationType, string>(new Dictionary<StandardAnimationType, string>
            {
                {StandardAnimationType.Idle, "Idle"},
                {StandardAnimationType.WalkingLeft, "Walk_Left"},
                {StandardAnimationType.JumpLeft, "Jump_Left"},
                {StandardAnimationType.JumpRight, "Jump_Right"},
                {StandardAnimationType.WalkingRight, "Walk_Right"}
            });

        private readonly List<AnimationDescriptor> animations_;
        public static AnimationFactory Instance => SINGLETON.Value;

        private AnimationFactory()
        {
            animations_ = AnimationDescriptors(CONTENT_PATH).Select(AnimationDescriptor.StaticLoad).ToList();
        }

        public static AnimationDescriptor AnimationFor(string category, StandardAnimationType animationType)
        {
            return AnimationFor(category, STANDARD_FILE_NAMES[animationType]);
        }

        public static AnimationDescriptor AnimationFor(string category, string animation)
        {
            return
                Instance.animations_
                    .First(descriptor => descriptor.Asset.Contains(category) && descriptor.Asset.Contains(animation));
        }

        private static IEnumerable<string> AnimationDescriptors(string folder)
        {
            var animationFiles =
                Directory.EnumerateFiles(folder)
                    .Where(
                        path =>
                            Path.HasExtension(path) &&
                            (Path.GetExtension(path)?.Equals(AnimationDescriptor.AnimationExtension) ?? false));
            var directories = Directory.EnumerateDirectories(folder);
            return animationFiles.Concat(directories.SelectMany(AnimationDescriptors));
        }
    }
}