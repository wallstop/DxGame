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
        IdleJump,
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
                {StandardAnimationType.IdleJump, "Idle_Jump"},
                {StandardAnimationType.JumpLeft, "Jump_Left"},
                {StandardAnimationType.JumpRight, "Jump_Right"},
                {StandardAnimationType.WalkingRight, "Walk_Right"}
            });

        private readonly Dictionary<string, AnimationDescriptor> animations_ =
            new Dictionary<string, AnimationDescriptor>();

        public static AnimationFactory Instance => SINGLETON.Value;

        private AnimationFactory()
        {
            foreach (var animationFile in AnimationDescriptors(CONTENT_PATH))
            {
                animations_[animationFile] = AnimationDescriptor.StaticLoad(animationFile);
            }
        }

        public static AnimationDescriptor AnimationFor(string category, StandardAnimationType animationType)
        {
            return AnimationFor(category, STANDARD_FILE_NAMES[animationType]);
        }

        public static AnimationDescriptor AnimationFor(string category, string animation)
        {
            return
                Instance.animations_
                    .First(
                        entry =>
                            (entry.Value.Asset.Contains(category) || entry.Key.Contains(category)) &&
                            (entry.Value.Asset.Contains(animation) || entry.Key.Contains(animation))).Value;
        }

        /**
            Recursively walk through all Content subdirectories looking for .adtr files
        */

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