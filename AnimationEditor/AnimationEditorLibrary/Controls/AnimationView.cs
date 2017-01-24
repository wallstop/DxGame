using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AnimationEditorLibrary.Core.Messaging;
using AnimationEditorLibrary.Core.Settings;
using AnimationEditorLibrary.EmptyKeysLib;
using AnimationEditorLibrary.EmptyKeysLib.Relay;
using AnimationEditorLibrary.Models;
using DxCore;
using DxCore.Core;
using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using WallNetCore.Cache.Advanced;
using WallNetCore.Validate;
using MouseEventArgs = EmptyKeys.UserInterface.Input.MouseEventArgs;
using MouseEventHandler = EmptyKeys.UserInterface.Input.MouseEventHandler;

namespace AnimationEditorLibrary.Controls
{
    internal enum BoundingBoxMovementMode
    {
        NotDoinIt,
        DoinIt,
        SpriteSheetMovin,
        Idk
    }

    public class AnimationView : ViewModelBase
    {
        private const bool RedrawTextures = true;
        private const float BaseScrollScale = 1200f;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private bool frameCountEnabled_;

        private int frameIndex_;

        private float scale_;

        private DxVector2 spriteSheetDrawOffset_;

        public string AssetPath { get; private set; }

        public string ContentDirectory
        {
            get { return Settings.ContentDirectory; }
            set
            {
                string oldValue = Settings.ContentDirectory;
                Settings.ContentDirectory = value;
                if(!Objects.Equals(oldValue, value))
                {
                    Logger.Info("Updated {0} to {1}", nameof(ContentDirectory), value);
                    // TODO: ... invalidate animation / ensure that's what we wanted to do?
                    RaisePropertyChanged();
                }
            }
        }

        public List<DxRectangle> CurrentFrameView
        {
            get
            {
                DxVector2 drawOffset;
                DxVector2 frameOffset;
                int width;
                int height;
                if(Descriptor.OffsetForFrame(FrameIndex, out frameOffset, out drawOffset, out width, out height))
                {
                    // TODO: Care about draw offset?
                    return new List<DxRectangle>(1)
                    {
                        new DxRectangle(frameOffset * Scale, width * Scale, height * Scale) + Offset() +
                        SpriteSheetDrawOffset
                    };
                }
                return new List<DxRectangle>(0);
            }
        }

        public bool FacingLeft
        {
            get { return Facing == Direction.West; }
            set
            {
                ChangeDirection(value ? Direction.West : Direction.East);
                RaisePropertyChanged();
            }
        }

        public bool FacingRight
        {
            get { return Facing == Direction.East; }
            set
            {
                ChangeDirection(value ? Direction.East : Direction.West);
                RaisePropertyChanged();
            }
        }

        public int FPS
        {
            get { return Descriptor.FramesPerSecond; }
            set
            {
                Builder.WithFps(value);
                RaisePropertyChanged();
                NotifyAnimationChanged();
            }
        }

        public int FrameCount
        {
            get { return Descriptor.FrameCount; }
            set
            {
                int oldFrameIndex = FrameIndex;
                int oldValue = Descriptor.FrameCount;
                Builder.WithFrameCount(value);

                if(oldValue < value && oldValue == 0)
                {
                    FrameIndex = 0;
                }
                else if(value < oldValue && FrameIndex == value)
                {
                    FrameIndex = value - 1;
                }
                else
                {
                    FrameIndex = oldFrameIndex;
                }

                RaisePropertyChanged();
                NotifyAnimationChanged(RedrawTextures);
            }
        }

        public bool FrameCountEnabled
        {
            get { return frameCountEnabled_; }
            set
            {
                frameCountEnabled_ = value;
                RaisePropertyChanged();
            }
        }

        public int FrameIndex
        {
            get { return frameIndex_; }
            set
            {
                frameIndex_ = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<FrameModel> Frames { get; }

        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
                new Dictionary<RoutedEvent, Delegate>
                {
                    [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(HandleMouseScroll),
                    [Mouse.MouseMoveEvent] = new MouseEventHandler(HandleMouseMove),
                    [Mouse.MouseDownEvent] = new MouseButtonEventHandler(HandleMouseDown),
                    [Mouse.MouseUpEvent] = new MouseButtonEventHandler(HandleMouseUp),
                    [Mouse.MouseLeaveEvent] = new MouseEventHandler(HandleMouseLeave)
                };

        public int Height
        {
            get { return Descriptor.Height; }
            set
            {
                Builder.WithHeight(value);
                RaisePropertyChanged();
                NotifyAnimationChanged(RedrawTextures);
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand NewCommand { get; }

        public Func<DxVector2> Offset { get; set; }

        public List<DxRectangle> OtherFrameView
        {
            get
            {
                int frameIndexToIgnore = FrameIndex;
                List<FrameDescriptor> frameDescriptors = Descriptor.Frames;
                List<DxRectangle> otherFrameView =
                    new List<DxRectangle>(frameDescriptors.Count == 0 ? 0 : frameDescriptors.Count - 1);
                for(int i = 0; i < frameDescriptors.Count; ++i)
                {
                    if(i == frameIndexToIgnore)
                    {
                        continue;
                    }
                    DxVector2 drawOffset;
                    DxVector2 frameOffset;
                    int width;
                    int height;
                    if(Descriptor.OffsetForFrame(i, out frameOffset, out drawOffset, out width, out height))
                    {
                        DxRectangle frameView = new DxRectangle(frameOffset * Scale, width * Scale, height * Scale) +
                                                Offset() + SpriteSheetDrawOffset;
                        otherFrameView.Add(frameView);
                    }
                }
                return otherFrameView;
            }
        }

        public ICommand SaveCommand { get; }

        // Ugh this coupling blows, pls make better
        public UniqueId ServiceId { get; set; }

        public ICommand SetContentDirectoryCommand { get; }

        public int Width
        {
            get { return Descriptor.Width; }
            set
            {
                Builder.WithWidth(value);
                NotifyAnimationChanged(RedrawTextures);
            }
        }

        private AnimationDescriptor.AnimationDescriptorBuilder Builder { get; }

        private Uri ContentDirectoryUri => new Uri(ContentDirectory);

        // Nullable
        private Texture2D CurrentTexture { get; set; }
        private AnimationDescriptor Descriptor => DescriptorCache.Get();

        private SingleElementLocalLoadingCache<AnimationDescriptor> DescriptorCache { get; }

        private Direction Facing { get; set; }

        // Overloaded - is either BoundingBox or SpriteSheet offset depending on MovementMode
        private DxVector2 LastOffsetStart { get; set; }

        private BoundingBoxMovementMode MovementMode { get; set; }

        private DxRectangle OffsetPlayspace
        {
            // Bounds area for the potential frame offsets
            get
            {
                if(Validate.Check.IsNull(CurrentTexture))
                {
                    return DxRectangle.EmptyRectangle;
                }

                int maxWidth = CurrentTexture.Width;
                int maxHeight = CurrentTexture.Height;
                return new DxRectangle(0, 0, maxWidth - Width, maxHeight - Height);
            }
        }

        private float Scale
        {
            get { return scale_; }
            set
            {
                scale_ = value;
                if(Validate.Check.IsNotNull(ServiceId))
                {
                    new ChangeScaleRequest(ServiceId, scale_).Emit();
                }
            }
        }

        private AnimationEditorSettings Settings { get; set; }

        private DxVector2 SpriteSheetDrawOffset
        {
            get { return spriteSheetDrawOffset_; }
            set
            {
                spriteSheetDrawOffset_ = value;
                new SpriteSheetOffsetChangedMessage(value).Emit();
            }
        }

        public AnimationView()
        {
            Builder = AnimationDescriptor.NewBuilder;
            FacingRight = true;

            SetContentDirectoryCommand = new RelayCommand(HandleSetContentDirectory);
            NewCommand = new RelayCommand(HandleNew);
            LoadCommand = new RelayCommand(HandleLoad);
            SaveCommand = new RelayCommand(HandleSave);
            Settings = new AnimationEditorSettings();
            Settings.Load();
            Scale = 1.0f;

            MovementMode = BoundingBoxMovementMode.NotDoinIt;
            Frames = new ObservableCollection<FrameModel>();
            DescriptorCache =
                new SingleElementLocalLoadingCache<AnimationDescriptor>(
                    CacheBuilder<FastCacheKey, AnimationDescriptor>.NewBuilder(), () => Builder.Build());
            Offset = () => new DxVector2();
        }

        public void OnClose()
        {
            try
            {
                Settings.Save();
            }
            catch(Exception e)
            {
                Logger.Error(e, "Caught unexpected exception while trying to save settings");
            }
        }

        private void ChangeDirection(Direction direction)
        {
            if(Facing == direction)
            {
                return;
            }
            Builder.WithOrientation(direction);
            new OrientationChangedMessage(direction).Emit();
            Facing = direction;
        }

        private bool CheckContentDirectorySet()
        {
            bool invalidContentDirectory = Validate.Check.IsNull(ContentDirectory);
            if(invalidContentDirectory)
            {
                const string errorMessage = "Content Directory is not set";
                LogError(errorMessage);
            }
            return !invalidContentDirectory;
        }

        private void HandleLoad(object whoCares)
        {
            if(!CheckContentDirectorySet())
            {
                return;
            }

            OpenFileDialog chooseAnimationDescriptor = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "AnimationDescriptor files(*.adtr)|*.adtr;",
                InitialDirectory = ContentDirectory
            };

            DialogResult animationSelectResult = chooseAnimationDescriptor.ShowDialog();
            switch(animationSelectResult)
            {
                case DialogResult.OK:
                {
                    AnimationDescriptor descriptor;
                    try
                    {
                        descriptor = AnimationDescriptor.StaticLoad(chooseAnimationDescriptor.FileName);
                    }
                    catch(Exception exception)
                    {
                        string errorMessage =
                            $"Unable to load {nameof(AnimationDescriptor)} from {chooseAnimationDescriptor.FileName}";
                        LogError(errorMessage, exception);
                        return;
                    }
                    // Now we need to check if the asset exists - hopefully it does
                    string hopefulAsset = ContentDirectory + descriptor.Asset;
                    // We need to find the actual file - monogame expects assets without extensions
                    DirectoryInfo assetDirectoryInfo = Directory.GetParent(hopefulAsset);
                    if(!assetDirectoryInfo.Exists)
                    {
                        string errorMessage = $"Failed to find asset directory for {chooseAnimationDescriptor.FileName}";
                        LogError(errorMessage);
                        return;
                    }
                    string fullDirectoryPath = assetDirectoryInfo.FullName;

                    // Builder.WithDescriptor(descriptor);

                    // TODO
                    break;
                }
                default:
                {
                    // TODO
                    break;
                }
            }

            // TODO
        }

        private void HandleMouseDown(object source, MouseButtonEventArgs mouseEventArgs)
        {
            DxVector2 uiCoordinates = mouseEventArgs.GetPosition().ToDxVector2();

            CameraService cameraService;
            if(!DxGame.Instance.ServiceProvider.TryGet(out cameraService))
            {
                string error = $"{typeof(CameraService)} does not exist, something is very wrong";
                Logger.Error(error);
                throw new DataException(error);
            }

            // Hit test to see if we're inside the current frame's bounding box
            DxVector2 worldCoordinates = cameraService.UiOffsetToWorldCoordinates(uiCoordinates);
            bool isBoundingBoxMove = CurrentFrameView.Any(_ => _.Contains(worldCoordinates));
            MovementMode = isBoundingBoxMove ? BoundingBoxMovementMode.DoinIt : BoundingBoxMovementMode.SpriteSheetMovin;

            Logger.Debug("Beginning move pattern for {0}", MovementMode);
            LastOffsetStart = mouseEventArgs.GetPosition().ToDxVector2();
        }

        private void HandleMouseLeave(object source, MouseEventArgs mouseEventArgs)
        {
            MovementMode = BoundingBoxMovementMode.NotDoinIt;
        }

        private void HandleMouseMove(object source, MouseEventArgs mouseEventArgs)
        {
            switch(MovementMode)
            {
                case BoundingBoxMovementMode.DoinIt:
                {
                    if(FrameIndex < 0)
                    {
                        // Can't do it without a selected Frame
                        return;
                    }
                    DxVector2 currentPosition = mouseEventArgs.GetPosition().ToDxVector2();
                    DxVector2 distance = currentPosition - LastOffsetStart;
                    distance /= Scale;
                    LastOffsetStart = currentPosition;

                    AnimationDescriptor animationDescriptor = DescriptorCache.Get();
                    FrameDescriptor currentFrame = animationDescriptor.Frames[FrameIndex];
                    currentFrame.FrameOffset += distance;
                    /* Bounded */
                    currentFrame.FrameOffset = currentFrame.FrameOffset.Bound(OffsetPlayspace);
                    Builder.WithFrame(FrameIndex, currentFrame);
                    NotifyAnimationChanged(RedrawTextures);
                    return;
                }
                case BoundingBoxMovementMode.SpriteSheetMovin:
                {
                    DxVector2 currentPosition = mouseEventArgs.GetPosition().ToDxVector2();
                    DxVector2 distance = currentPosition - LastOffsetStart;
                    LastOffsetStart = currentPosition;

                    SpriteSheetDrawOffset += distance;
                    break;
                }
                case BoundingBoxMovementMode.NotDoinIt:
                {
                    // I ain't doin this
                    return;
                }
                default:
                {
                    Logger.Debug("Somehow ended up with {0} of {1}", nameof(MovementMode), MovementMode);
                    return;
                }
            }
        }

        private void HandleMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            Scale = MathHelper.Clamp(Scale, 0.1f, float.MaxValue);
            Builder.WithScale(Scale);
            NotifyAnimationChanged();
        }

        private void HandleMouseUp(object source, MouseButtonEventArgs mouseEventArgs)
        {
            MovementMode = BoundingBoxMovementMode.NotDoinIt;
        }

        private void HandleNew(object whoCares)
        {
            if(!CheckContentDirectorySet())
            {
                return;
            }

            OpenFileDialog chooseSpriteSheet = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Multiselect = false,
                Filter = "Image files(*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = ContentDirectory
            };

            DialogResult spriteSelectResult = chooseSpriteSheet.ShowDialog();
            switch(spriteSelectResult)
            {
                case DialogResult.OK:
                {
                    string assetPath = chooseSpriteSheet.FileName;
                    Validate.Hard.IsNotNull(assetPath);
                    Uri assetAsUri = new Uri(assetPath);
                    if(!ContentDirectoryUri.IsBaseOf(assetAsUri))
                    {
                        IMessageBoxService messageBoxService = GetService<IMessageBoxService>();
                        messageBoxService?.Show($"{assetPath} is not a subdirectory of the current content directory",
                            Nop.Instance, false);
                        return;
                    }
                    Uri assetAsRelative = ContentDirectoryUri.MakeRelativeUri(assetAsUri);
                    string assetAsRelativeString = assetAsRelative.ToString();
                    // Now we need to strip the file extension for compatibility with monogame's content manager
                    // I am a poor man, forgive my sins
                    int extensionIndex = assetAsRelativeString.LastIndexOf(".");
                    string assetWithoutExtension = assetAsRelativeString.Substring(0, extensionIndex);
                    Builder.ResetToBase();
                    Builder.WithAsset(assetWithoutExtension);
                    AssetPath = assetPath;
                    FrameCountEnabled = true;
                    NotifyAnimationChanged();
                    Logger.Info("Updated asset to {0}", assetPath);
                    break;
                }
                default:
                {
                    Logger.Error("Error selecting sprite sheet, dialogResult: {0}", spriteSelectResult);
                    break;
                }
            }

            // TODO
        }

        private void HandleSave(object whoCares)
        {
            if(!CheckContentDirectorySet())
            {
                return;
            }

            SaveFileDialog saveAnimation = new SaveFileDialog
            {
                CreatePrompt = false,
                OverwritePrompt = true,
                AddExtension = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Filter = "AnimationDescriptor files(*.adtr)|*.adtr;",
                InitialDirectory = ContentDirectory
            };

            DialogResult saveResult = saveAnimation.ShowDialog();
            switch(saveResult)
            {
                case DialogResult.OK:
                {
                    try
                    {
                        Descriptor.Save(saveAnimation.FileName);
                        Logger.Info("{0} saved successfully", saveAnimation.FileName);
                    }
                    catch(Exception e)
                    {
                        Logger.Error(e, "Unexpected error encountered while saving {0}", saveAnimation.FileName);
                    }
                    break;
                }
                default:
                {
                    Logger.Error("Error saving animation file, dialogResult: {0}", saveResult);
                    break;
                }
            }
        }

        private void HandleSetContentDirectory(object whoCares)
        {
            string oldContentDirectory = ContentDirectory;
            FolderBrowserDialog chooseContentDirectory = new FolderBrowserDialog
            {
                Description = "Content directory for main DxGame",
                ShowNewFolderButton = false
            };

            DialogResult chooseContentResult = chooseContentDirectory.ShowDialog();
            switch(chooseContentResult)
            {
                case DialogResult.OK:
                {
                    ContentDirectory = chooseContentDirectory.SelectedPath + Path.DirectorySeparatorChar;
                    Logger.Debug($"{0} updated to {1}", nameof(ContentDirectory), ContentDirectory);
                    break;
                }
                default:
                {
                    Logger.Debug("Ignoring {0} update with selection result {1}", nameof(ContentDirectory),
                        chooseContentResult);
                    break;
                }
            }
            if(!Objects.Equals(oldContentDirectory, ContentDirectory))
            {
                Builder.ResetToBase();
                NotifyAnimationChanged();
            }
        }

        private void LogError(string errorMessage, Exception exception = null)
        {
            if(!ReferenceEquals(exception, null))
            {
                Logger.Error(exception, errorMessage);
            }
            else
            {
                Logger.Error(errorMessage);
            }
            GetService<IMessageBoxService>()?.Show(errorMessage, Nop.Instance, false);
        }

        private void NotifyAnimationChanged(bool clearTextures = false)
        {
            DescriptorCache.Invalidate();
            new AnimationChangedMessage(AssetPath, Descriptor).Emit();

            if(!clearTextures)
            {
                return;
            }

            Frames.Clear();
            if(Validate.Check.IsNull(AssetPath))
            {
                return;
            }

            using(Stream textureStream = File.Open(AssetPath, FileMode.Open))
            {
                CurrentTexture = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, textureStream);
                foreach(FrameDescriptor frame in Descriptor.Frames)
                {
                    FrameModel frameModel = new FrameModel(CurrentTexture,
                        new DxRectangle(frame.FrameOffset, Width, Height));
                    Frames.Add(frameModel);
                }
            }
        }
    }
}