using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AnimationEditor.Core;
using AnimationEditor.Extension;
using DxCore.Core.Animation;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace AnimationEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Thread animationRunner_;

        public string ContentDirectory => AnimationSettings.ContentDirectory;

        public int FrameIndex { get; set; }

        public ObservableCollection<Image> Frames { get; }

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

        private AnimationDescriptor AnimationDescriptor { get; set; }

        private AnimationSettings AnimationSettings { get; set; }

        private static string AnimationSettingsFile { get; } = "AnimationSettings";

        private bool ContentDirectorySet => !string.IsNullOrWhiteSpace(AnimationSettings.ContentDirectory);

        private AnimationFrameOffset.AnimationFrameOffsetBuilder FrameOffsetBuilder { get; }

        private Size FrameSize
            => new Size(AnimationDescriptor.BoundingBox.Width, AnimationDescriptor.BoundingBox.Height);

        public MainWindow()
        {
            FrameOffsetBuilder = new AnimationFrameOffset.AnimationFrameOffsetBuilder();
            InitializeComponent();

            animationRunner_ = new Thread(AnimationPreview);
            animationRunner_.Start();

            try
            {
                AnimationSettings =
                    AnimationSettings.StaticLoad(AnimationSettingsFile + AnimationSettings.SettingsExtension);
            }
            catch
            {
                AnimationSettings = new AnimationSettings();
            }

            AnimationDescriptor = AnimationDescriptor.Empty();
            Frames = new ObservableCollection<Image>();
            DataContext = this;
            Closing += OnExit;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AnimationPreview()
        {
            /*
            while(true)
            {
                // TODO: Hot-sleep for correct amount of time
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    animationTransform_ = new MatrixTransform();
                    GC.Collect();
                }));
            }
            */
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
            AnimationDescriptor = new AnimationDescriptor(); // Easiest way to reset
            Frames.Clear();
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
                        Image newFrame = new Image {Source = Source.Source, Clip = boundary};
                        Frames.Add(newFrame);
                    }
                    catch
                    {
                        // Ignore
                    }
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
            }

            FrameOffsetBuilder.WithHeight(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void HandleLoad(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            // TODO: Load .adtr file into state
            Clear();
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
            }

            FrameOffsetBuilder.WithWidth(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void OnExit(object sender, CancelEventArgs eventArgs)
        {
            try
            {
                AnimationSettings.Save(AnimationSettingsFile + AnimationSettings.SettingsExtension);
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