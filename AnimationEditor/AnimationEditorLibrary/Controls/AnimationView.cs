using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AnimationEditorLibrary.Core.Messaging;
using AnimationEditorLibrary.EmptyKeys.Relay;
using DxCore.Core.Animation;
using DxCore.Core.Messaging;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using NLog;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Controls
{
    public class AnimationView : ViewModelBase
    {
        private const float BaseScrollScale = 1200f;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string contentDirectory_;

        public string ContentDirectory
        {
            get { return contentDirectory_; }
            set
            {
                contentDirectory_ = value;
                RaisePropertyChanged();
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
                Builder.WithFrameCount(value);
                RaisePropertyChanged();
                NotifyAnimationChanged();
            }
        }

        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
                new Dictionary<RoutedEvent, Delegate>
                {
                    [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(HandleMouseScroll)
                };

        public int Height
        {
            get { return Descriptor.Height; }
            set
            {
                Builder.WithHeight(value);
                RaisePropertyChanged();
                NotifyAnimationChanged();
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand SaveCommand { get; }

        public ICommand SetContentDirectoryCommand { get; }

        public int Width
        {
            get { return Descriptor.Width; }
            set
            {
                Builder.WithWidth(value);
                NotifyAnimationChanged();
            }
        }

        private AnimationDescriptor.AnimationDescriptorBuilder Builder { get; }

        private Uri ContentDirectoryUri => new Uri(ContentDirectory);
        private AnimationDescriptor Descriptor => Builder.Build();

        private Direction Facing { get; set; }

        private float Scale { get; set; }

        public AnimationView()
        {
            Builder = AnimationDescriptor.NewBuilder;
            FacingRight = true;

            SetContentDirectoryCommand = new RelayCommand(HandleSetContentDirectory);
            NewCommand = new RelayCommand(HandleNew);
            LoadCommand = new RelayCommand(HandleLoad);
            SaveCommand = new RelayCommand(HandleSave);
        }

        private void ChangeDirection(Direction direction)
        {
            if(Facing == direction)
            {
                return;
            }
            new OrientationChangedMessage(direction).Emit();
            Facing = direction;
        }

        private bool CheckContentDirectorySet()
        {
            bool invalidContentDirectory = Validate.Check.IsNull(ContentDirectory);
            if(invalidContentDirectory)
            {
                IMessageBoxService messageBoxService = GetService<IMessageBoxService>();

                const string errorMessage = "Content Directory is not set";
                if(!Validate.Check.IsNull(messageBoxService))
                {
                    messageBoxService.Show(errorMessage, Nop.Instance, false);
                }
                Logger.Error(errorMessage);
            }
            return !invalidContentDirectory;
        }

        private void HandleLoad(object whoCares)
        {
            if(!CheckContentDirectorySet()) {}
            // TODO
        }

        private void HandleMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            Builder.WithScale(Scale);
            NotifyAnimationChanged();
        }

        private void HandleNew(object whoCares)
        {
            if(!CheckContentDirectorySet())
            {
                return;
            }

            OpenFileDialog chooseSpriteSheet = new OpenFileDialog();
            chooseSpriteSheet.CheckFileExists = true;
            chooseSpriteSheet.Multiselect = false;
            chooseSpriteSheet.Filter = "Image files(*.png"

            // TODO
        }

        private void HandleSave(object whoCares)
        {
            SaveFileDialog saveAnimation = new SaveFileDialog
            {
                CreatePrompt = false,
                OverwritePrompt = true,
                AddExtension = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Filter = "AnimationDescriptor files(*.adtr)|*.adtr;"
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
                    ContentDirectory = chooseContentDirectory.SelectedPath;
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
            if(!oldContentDirectory.Equals(ContentDirectory))
            {
                Builder.ResetToBase();
                NotifyAnimationChanged();
            }
        }

        private void NotifyAnimationChanged()
        {
            new AnimationChangedMessage(Descriptor).Emit();
        }
    }
}