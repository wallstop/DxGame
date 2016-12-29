using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AnimationEditor.Core;
using AnimationEditor.Extension;
using DxCore.Core.Animation;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace AnimationEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Thread animationRunner_;

        public AnimationDescriptor AnimationDescriptor { get; }

        public string ContentDirectory => AnimationSettings.ContentDirectory;
        public int Fps => AnimationDescriptor?.FramesPerSecond ?? 0;
        public int FrameCount => AnimationDescriptor?.FrameCount ?? 0;
        public int FrameHeight => (int) AnimationDescriptor.BoundingBox.Height;

        public int FrameIndex { get; set; }

        public ObservableCollection<Image> Frames { get; }

        public int FrameWidth => (int) AnimationDescriptor.BoundingBox.Width;

        public ICommand LoadCommand
        {
            get
            {
                return new RelayCommand(_ =>
                {
                    // TODO: Need to figure out how to find the asset without a file extension
                });
            }
        }

        public ICommand SetContentDirectory
        {
            // Could cache this, but who cares
            get
            {
                return new RelayCommand(_ =>
                {
                    FolderBrowserDialog chooseContentDirectory = new FolderBrowserDialog
                    {
                        Description = @"Select the DxGame Content directory to use"
                    };
                    DialogResult result = chooseContentDirectory.ShowDialog(this.GetIWin32Window());
                    switch(result)
                    {
                        case System.Windows.Forms.DialogResult.OK:
                        {
                            AnimationSettings.ContentDirectory = chooseContentDirectory.SelectedPath;
                            OnPropertyChanged(nameof(ContentDirectory));
                            break;
                        }
                        // TODO: Handle other cases, don't care enough right now
                        default:
                        {
                            break;
                        }
                    }
                });
            }
        }

        private AnimationSettings AnimationSettings { get; set; }

        private bool ContentDirectorySet => !string.IsNullOrWhiteSpace(AnimationSettings.ContentDirectory);

        private AnimationFrameOffset.AnimationFrameOffsetBuilder FrameOffsetBuilder { get; }

        private Size FrameSize
            => new Size(AnimationDescriptor.BoundingBox.Width, AnimationDescriptor.BoundingBox.Height);

        private Point? LastDragLocation { get; set; }

        public MainWindow()
        {
            FrameOffsetBuilder = new AnimationFrameOffset.AnimationFrameOffsetBuilder();
            InitializeComponent();

            animationRunner_ = new Thread(AnimationPreview);
            animationRunner_.Start();

            AnimationSettings = new AnimationSettings();
            AnimationSettings.Load();

            AnimationDescriptor = AnimationDescriptor.Empty();
            Frames = new ObservableCollection<Image>();
            DataContext = this;
            Closing += OnExit;

            SourceCanvas.MouseEnter += HandleMouseEnterImage;
            SourceCanvas.MouseLeave += HandleMouseLeaveImage;
            SourceCanvas.MouseMove += HandleMouseMoveImage;
            SourceCanvas.MouseLeftButtonDown += HandleMouseDownImage;
            SourceCanvas.MouseLeftButtonUp += HandleMouseUpImage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AnimationPreview()
        {
            Stopwatch animationTimer = Stopwatch.StartNew();
            int frame = 0;
            while(true)
            {
                TimeSpan tick = animationTimer.Elapsed;
                // TODO: Hot-sleep for correct amount of time
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    DxVector2 drawOffset;
                    DxVector2 frameOffset;
                    DxRectangle boundingBox;
                    if(AnimationDescriptor.FrameOffsets.OffsetForFrame(frame, out frameOffset, out drawOffset,
                        out boundingBox))
                    {
                        Animation.Source = Source.Source;
                        Rect crop = new Rect
                        {
                            X = frameOffset.X,
                            Y = frameOffset.Y,
                            Width = boundingBox.Width,
                            Height = boundingBox.Height
                        };
                        Animation.Clip = new RectangleGeometry(crop);
                    }
                }));
                while(animationTimer.Elapsed - tick < TimeSpan.FromMilliseconds(1000.0 / Math.Max(1.0, Fps)))
                {
                    // Hot sleep, newing a Timespan every time to get the most accurate values :)
                }
                frame = frame.WrappedAdd(1, FrameCount == 0 ? 1 : FrameCount);
            }
        }

        private void CheckClose(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        private void CheckNew(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        private void CheckSave(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }

        private void Clear()
        {
            AnimationDescriptor.ResetToBase();
            Frames.Clear();
            OnPropertyChanged(nameof(Fps));
            OnPropertyChanged(nameof(FrameWidth));
            OnPropertyChanged(nameof(FrameHeight));
            OnPropertyChanged(nameof(FrameCount));
        }

        private void HandleClose(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            animationRunner_.Abort();
            Close();
        }

        private void HandleFpsChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            AnimationDescriptor.FramesPerSecond = newValue;
            // TODO: Handle synch with animation preview?
        }

        private void HandleFrameCountChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            /* We baleeted some frames, we need to remove them */
            if(newValue < oldValue)
            {
                for(int i = newValue; i < oldValue; ++i)
                {
                    FrameOffsetBuilder.WithoutFrameOffset(i);
                    Frames.RemoveAt(i);
                }
            }
            else
            {
                for(int i = oldValue; i < newValue; ++i)
                {
                    FrameOffsetBuilder.WithFrameOffset(i, AnimationDescriptor.NewFrameDescriptor);
                    try
                    {
                        Point frameOrigin = new Point(0, 0);
                        Rect frameBounds = new Rect(frameOrigin, FrameSize);

                        RectangleGeometry boundary = new RectangleGeometry(frameBounds);
                        Image newFrame = new Image
                        {
                            Source = Source.Source,
                            Clip = boundary,
                            ClipToBounds = true,
                            Stretch = Stretch.None,
                            ToolTip = (i + 1).ToString()
                            // Fake a good image by sliding it backwards so we just see our tiny little dooderoo
                        };

                        Frames.Add(newFrame);
                    }
                    catch
                    {
                        // Ignore
                    }
                }
                if(FrameIndex <= 0)
                {
                    FrameIndex = 0;
                    OnPropertyChanged(nameof(FrameIndex));
                }
            }

            AnimationDescriptor.FrameCount = newValue;
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void HandleHeightChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            foreach(Image frame in Frames)
            {
                RectangleGeometry existingGeometry = (RectangleGeometry) frame.Clip;
                Size updatedSize = new Size(existingGeometry.Rect.Width, newValue);
                existingGeometry.Rect = new Rect(existingGeometry.Rect.Location, updatedSize);
                frame.RenderSize = updatedSize;
            }

            FrameOutline.Height = newValue;

            FrameOffsetBuilder.WithHeight(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void HandleLoad(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            // TODO: Load .adtr file into state
            Clear();
        }

        private void HandleMouseDownImage(object sender, MouseButtonEventArgs eventArgs)
        {
            LastDragLocation = eventArgs.GetPosition(Source);
        }

        private void HandleMouseEnterImage(object sender, MouseEventArgs eventArgs)
        {
            // TODO, don't care right now
        }

        private void HandleMouseLeaveImage(object sender, MouseEventArgs eventArgs)
        {
            LastDragLocation = null;
        }

        private void HandleMouseMoveImage(object sender, MouseEventArgs eventArgs)
        {
            if(!LastDragLocation.HasValue)
            {
                return;
            }
            Point oldLocation = LastDragLocation.Value;
            Point newLocation = eventArgs.GetPosition(Source);
            LastDragLocation = newLocation;

            Vector offset = newLocation - oldLocation;

            RectangleGeometry frameBounds = (RectangleGeometry) Frames[FrameIndex].Clip;
            Rect oldBounds = frameBounds.Rect;
            oldBounds.X += offset.X;
            oldBounds.Y += offset.Y;
            frameBounds.Rect = oldBounds;

            TranslateTransform translation = new TranslateTransform {X = oldBounds.X, Y = oldBounds.Y};
            FrameDescriptor currentFrameDescriptor = AnimationDescriptor.FrameOffsets.Offsets[FrameIndex];

            currentFrameDescriptor.FrameOffset = new DxVector2(oldBounds.X, oldBounds.Y);
            FrameOffsetBuilder.WithFrameOffset(FrameIndex, currentFrameDescriptor);

            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();

            FrameOutline.RenderTransform = translation;
        }

        private void HandleMouseUpImage(object sender, MouseButtonEventArgs eventArgs)
        {
            LastDragLocation = null;
        }

        private void HandleNew(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if(!ContentDirectorySet)
            {
                MessageBox.Show(this, "Content Directory required before assets can be loaded");
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Image Files|*.jpeg;*.png;*.jpg;*.gif|All Files|*.*",
                InitialDirectory = AnimationSettings.ContentDirectory
            };

            bool? result = openFile.ShowDialog();
            if(result != true)
            {
                return;
            }

            string fileName = openFile.FileName;
            Uri selectedFile = new Uri(fileName);
            Uri contentDirectory = new Uri(AnimationSettings.ContentDirectory + "/");
            if(!contentDirectory.IsBaseOf(selectedFile))
            {
                MessageBox.Show(this, $"{fileName} is not in the content directory - please try again");
                return;
            }

            try
            {
                Uri relativeAssetPath = contentDirectory.MakeRelativeUri(selectedFile);
                string assetWithoutExtension = Path.ChangeExtension(relativeAssetPath.OriginalString, null);

                BitmapImage sourceImage = new BitmapImage();
                sourceImage.BeginInit();
                sourceImage.UriSource = new Uri(fileName);
                sourceImage.EndInit();

                Source.Source = sourceImage;
                // Clear out any preconceptions
                Clear();
                AnimationDescriptor.Asset = assetWithoutExtension;
            }
            catch
            {
                MessageBox.Show(this, $"Error opening {fileName}");
            }
        }

        private void HandleSave(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                DefaultExt = AnimationDescriptor.AnimationExtension,
                Filter = $"Animation Descriptor|{AnimationDescriptor.AnimationExtension}"
            };

            bool? result = saveFile.ShowDialog();
            if(result == false)
            {
                return;
            }

            string fileName = saveFile.FileName;
            try
            {
                AnimationDescriptor.Save(fileName);
            }
            catch
            {
                MessageBox.Show(this, $"Error saving {fileName}");
            }
        }

        private void HandleWidthChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            foreach(Image frame in Frames)
            {
                RectangleGeometry existingGeometry = (RectangleGeometry) frame.Clip;
                Size updatedSize = new Size(newValue, existingGeometry.Rect.Height);
                existingGeometry.Rect = new Rect(existingGeometry.Rect.Location, updatedSize);
                frame.RenderSize = updatedSize;
            }

            FrameOutline.Width = newValue;
            FrameOffsetBuilder.WithWidth(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void OnExit(object sender, CancelEventArgs eventArgs)
        {
            try
            {
                AnimationSettings.Save();
            }
            catch
            {
                // LOL ignore
            }
        }

        private void ValidateNumericInput(object sender, TextCompositionEventArgs eventArgs)
        {
            bool numeric = true;
            foreach(char character in eventArgs.Text)
            {
                numeric = numeric && char.IsNumber(character);
            }
            eventArgs.Handled = !numeric;
        }
    }
}