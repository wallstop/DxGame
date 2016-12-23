using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AnimationEditor.Extension;
using DxCore.Core.Animation;
using Microsoft.Win32;

namespace AnimationEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Thread animationRunner_;

        private Transform animationTransform_;
        private AnimationDescriptor AnimationDescriptor { get; set; } = new AnimationDescriptor();

        private Transform AnimationTransform
        {
            get { return animationTransform_; }
            set
            {
                animationTransform_ = value;
                OnPropertyChanged();
            }
        }

        private AnimationFrameOffset.AnimationFrameOffsetBuilder FrameOffsetBuilder { get; }

        public MainWindow()
        {
            FrameOffsetBuilder = new AnimationFrameOffset.AnimationFrameOffsetBuilder();
            InitializeComponent();

            animationRunner_ = new Thread(AnimationPreview);
            animationRunner_.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AnimationPreview()
        {
            // while(true)
            // {
            // TODO: Hot-sleep for correct amount of time
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            //    new Action(() => { animationTransform_ = new MatrixTransform(); }));
            // }
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
        }

        private void HandleFrameCountChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            if(newValue < oldValue)
            {
                for(int i = newValue; i < oldValue; ++i)
                {
                    FrameOffsetBuilder.WithoutFrameOffset(i);
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

            FrameOffsetBuilder.WithHeight(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
        }

        private void HandleNew(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Image Files|*.jpeg;*.png;*.jpg;*.gif|All Files|*.*"
            };

            bool? result = openFile.ShowDialog();
            if(result == true)
            {
                string fileName = openFile.FileName;
                try
                {
                    BitmapImage sourceImage = new BitmapImage();
                    sourceImage.BeginInit();
                    sourceImage.UriSource = new Uri(fileName);
                    sourceImage.EndInit();

                    Source.Source = sourceImage;
                }
                catch
                {
                    MessageBox.Show(this, $"Error opening {fileName}");
                }
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
            if(result == true)
            {
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
        }

        private void HandleWidthChanged(object sender, RoutedPropertyChangedEventArgs<object> eventArgs)
        {
            int oldValue = eventArgs.ExtractOldValue(0);
            int newValue = eventArgs.ExtractNewValue(0);
            if(oldValue == newValue)
            {
                return;
            }

            FrameOffsetBuilder.WithWidth(newValue);
            AnimationDescriptor.FrameOffsets = FrameOffsetBuilder.Build();
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