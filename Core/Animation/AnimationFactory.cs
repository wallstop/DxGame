using DXGame.Core.Utils;
using DXGame.Main;
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

        private readonly UnboundedCache<string, AnimationDescriptor> animationCache_ = new UnboundedCache<string, AnimationDescriptor>();

        public static AnimationFactory Instance => SINGLETON.Value;

        private AnimationFactory()
        {
            foreach (var animationFile in AnimationDescriptors(DxGame.Instance.Content.RootDirectory))
            {
                animationCache_.PutIfAbsent(animationFile, AnimationDescriptor.StaticLoad(animationFile));
            }
        }

        public static AnimationDescriptor AnimationFor(string category, StandardAnimationType animationType)
        {
            return AnimationFor(category, STANDARD_FILE_NAMES[animationType]);
        }

        public static AnimationDescriptor AnimationFor(string category, string animation)
        {
            AnimationDescriptor animationDescriptor = 
                Instance.animationCache_.KeyedElements
                    .First(
                        entry =>
                            (entry.Value.Asset.Contains(category) || entry.Key.Contains(category)) &&
                            (entry.Value.Asset.Contains(animation) || entry.Key.Contains(animation))).Value;
            return animationDescriptor;
        }

        public static void GenerateStaticStandardAnimationTypes(string 

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