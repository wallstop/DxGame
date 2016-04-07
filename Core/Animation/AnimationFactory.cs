using DXGame.Core.Utils;
using DXGame.Main;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DXGame.Core.Utils.Cache;
using DXGame.Core.Utils.Cache.Simple;

namespace DXGame.Core.Animation
{
    public enum StandardAnimationType
    {
        Idle,
        Moving,
        Jumping
    }

    public class AnimationFactory
    {
        private static readonly ReadOnlyCollection<string> ASSET_FILE_EXTENSIONS = new ReadOnlyCollection<string>(new List<string> { ".png" });
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<AnimationFactory> SINGLETON =
            new Lazy<AnimationFactory>(() => new AnimationFactory());

        private static readonly ReadOnlyDictionary<StandardAnimationType, string> STANDARD_FILE_NAMES =
            new ReadOnlyDictionary<StandardAnimationType, string>(new Dictionary<StandardAnimationType, string>
            {
                {StandardAnimationType.Idle, "Idle"},
                {StandardAnimationType.Moving, "Moving"},
                {StandardAnimationType.Jumping, "Jumping"}
            });

        private readonly UnboundedSimpleCache<string, AnimationDescriptor> animationSimpleCache_ = new UnboundedSimpleCache<string, AnimationDescriptor>();
        private readonly UnboundedLoadingSimpleCache<string, bool> generatedStaticAnimations_ = new UnboundedLoadingSimpleCache<string, bool>(InternalGenerateStaticStandardAnimationTypes);
        /* TODO: Content-directory enumeration caching */
        //private readonly List<string> allContentFiles_;

        public static AnimationFactory Instance => SINGLETON.Value;

        private AnimationFactory()
        {
            foreach (var animationFile in AnimationDescriptors(DxGame.Instance.Content.RootDirectory))
            {
                animationSimpleCache_.PutIfAbsent(animationFile, AnimationDescriptor.StaticLoad(animationFile));
            }
        }

        public static AnimationDescriptor AnimationFor(string category, StandardAnimationType animationType)
        {
            return AnimationFor(category, STANDARD_FILE_NAMES[animationType]);
        }

        public static AnimationDescriptor AnimationFor(string category, string animation)
        {
            AnimationDescriptor animationDescriptor = SearchCache(category, animation);
            if(Check.IsNullOrDefault(animationDescriptor))
            {
                LOG.Info($"Found no {typeof(AnimationDescriptor)} for {category}, {animation}, attempting to generate static files");
                GenerateStaticStandardAnimationTypes(category);
                animationDescriptor = SearchCache(category, animation);
            }
            Validate.IsNotNullOrDefault(animationDescriptor, $"Could not find a {typeof(AnimationDescriptor)} for {category}, {animation}");
            return animationDescriptor;
        }

        private static AnimationDescriptor SearchCache(string category, string animation)
        {
            AnimationDescriptor animationDescriptor =
                Instance.animationSimpleCache_.KeyedElements
                    .FirstOrDefault(
                        entry =>
                            (entry.Value.Asset.Contains(category) || entry.Key.Contains(category)) &&
                            (entry.Value.Asset.Contains(animation) || entry.Key.Contains(animation))).Value;
            return animationDescriptor;
        }

        /**
            <summary>
                Creating all of the necessary standard animation files for a single, static (test) asset is incredibly annoying!
                Wouldn't it be great if there was a method that just generated them all for you (but didn't save them out to disk so
                source control doesn't get polluted?)

                Well, that's what this does. If you provide it the name of an asset file, it will traverse the Content tree
                recursively and attempt to find it. If it can't find it, or finds duplicates, it will log the result and return false.
                Otherwise, it will generate and add "fake" AnimationDescriptors consisting of 1-frame animations of the provided asset
                file to the cache.
            </summary>
        */
        public static bool GenerateStaticStandardAnimationTypes(string animationFile)
        {
            return Instance.generatedStaticAnimations_.Get(animationFile);
        }

        private static bool InternalGenerateStaticStandardAnimationTypes(string animationFile)
        {
            IEnumerable<string> assetFiles = ContentFiles(DxGame.Instance.Content.RootDirectory);
            List<string> matchingAssetFiles = assetFiles.Where(assetFile => Objects.Equals(animationFile, Path.GetFileNameWithoutExtension(assetFile))).ToList();
            if(matchingAssetFiles.Count() != 1)
            {
                LOG.Info($"Found {matchingAssetFiles.Count()} asset files that matched {animationFile} ({matchingAssetFiles}) - cannot generate StandardAnimationTypes");
                return false;
            }
            string pathToAssetFile = matchingAssetFiles.First();
            pathToAssetFile = StripContentDirectory(pathToAssetFile);
            foreach(StandardAnimationType animationType in Enum.GetValues(typeof(StandardAnimationType)))
            {
                AnimationDescriptor fakeDescriptor = new AnimationDescriptor();
                fakeDescriptor.Asset = pathToAssetFile;
                fakeDescriptor.FrameCount = 1;
                fakeDescriptor.FramesPerSecond = 60;
                fakeDescriptor.Scale = 1.0;
                string mangledFakePath = ManipulatePathToIncludeAnimationType(pathToAssetFile, animationType);
                Instance.animationSimpleCache_.PutIfAbsent(mangledFakePath, fakeDescriptor);
            }
            return true;
        }

        /**
            <summary>
                Removes the Content directory and any directory separators from a path, if the path begins with them.
                This is useful for when we find files, but expect the game to load them (they will be in the form Content\\MyFile.png, but 
                the game's content is looking for "MyFile.png")
            </summary>
        */
        private static string StripContentDirectory(string path)
        {
            if(path.StartsWith(DxGame.Instance.Content.RootDirectory))
            {
                path = path.Substring(DxGame.Instance.Content.RootDirectory.Length);
            }
            while(path.StartsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Substring(Path.DirectorySeparatorChar.ToString().Length);
            }
            return path;
        }

        /**
            <summary>
                We do hazy matching on animations in order to provide an easy interface. While its kind of hacky,
                this method will generate an expected animation description file for the animation type and asset file
            </summary>
        */
        private static string ManipulatePathToIncludeAnimationType(string pathToAssetFile, StandardAnimationType animationType)
        {
            string extension = Path.GetExtension(pathToAssetFile);
            int lastNonExtensionIndex = pathToAssetFile.LastIndexOf(extension);
            string fullPathWithoutExtension = pathToAssetFile.Substring(0, lastNonExtensionIndex);
            return $"{fullPathWithoutExtension}_{STANDARD_FILE_NAMES[animationType]}{AnimationDescriptor.AnimationExtension}";
        }

        /**
            <summary>
                Returns a full enumeration of all content files in the provided 
            </summary>
        */
        private static IEnumerable<string> ContentFiles(string directory)
        {
            IEnumerable<string> assetFiles = Directory.EnumerateFiles(directory).Where(path => Path.HasExtension(path) && ASSET_FILE_EXTENSIONS.Contains(Path.GetExtension(path)));
            IEnumerable<string> subdirectories = Directory.EnumerateDirectories(directory);
            return assetFiles.Concat(subdirectories.SelectMany(ContentFiles));
        }

        /**
            Recursively walk through all Content subdirectories looking for .adtr files
        */

        private static IEnumerable<string> AnimationDescriptors(string directory)
        {
            var animationFiles =
                Directory.EnumerateFiles(directory)
                    .Where(
                        path =>
                            Path.HasExtension(path) &&
                            (Path.GetExtension(path)?.Equals(AnimationDescriptor.AnimationExtension) ?? false));
            var directories = Directory.EnumerateDirectories(directory);
            return animationFiles.Concat(directories.SelectMany(AnimationDescriptors));
        }
    }
}