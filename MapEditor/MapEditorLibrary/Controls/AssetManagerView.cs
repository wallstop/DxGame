using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DxCore;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Camera;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using MapEditorLibrary.Core.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using MouseEventArgs = EmptyKeys.UserInterface.Input.MouseEventArgs;
using MouseEventHandler = EmptyKeys.UserInterface.Input.MouseEventHandler;

namespace MapEditorLibrary.Controls
{
    internal enum TilePloppinMode
    {
        None,
        Ploppin,
        Deletin
    }

    public class AssetManagerView : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const float BaseScrollScale = 1200f;

        private object selectedTile_;
        private float scale_ = 1.0f;

        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }

        public Tile SelectedTile
        {
            get
            {
                if(ReferenceEquals(selectedTile_, null))
                {
                    return null;
                }
                TileModel selectedTile = SelectedTileModel;
                if(ReferenceEquals(selectedTile, null))
                {
                    Logger.Warn("{0} was not a {1} (was {2}, {3})", selectedTile_, typeof(TileModel),
                        selectedTile_.GetType(), selectedTile_);
                    return null;
                }
                return new Tile(SelectedTileType, selectedTile.Texture.Name);
            }
        }

        public TileModel SelectedTileModel => selectedTile_ as TileModel;

        public object SelectedBlock
        {
            get { return selectedTile_; }
            set
            {
                SelectedTileType = TileType.Block;
                SetProperty(ref selectedTile_, value);
            }
        }

        public object SelectedPlatform
        {
            get { return selectedTile_; }
            set
            {
                SelectedTileType = TileType.Platform;
                SetProperty(ref selectedTile_, value);
            }
        }

        public TileType SelectedTileType { get; private set; } = TileType.None;

        public ObservableCollection<TileModel> Blocks { get; }
        public ObservableCollection<TileModel> Platforms { get; }

        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
                new Dictionary<RoutedEvent, Delegate>
                {
                    [Mouse.MouseDownEvent] = new MouseButtonEventHandler(OnMouseDown),
                    [Mouse.MouseMoveEvent] = new MouseEventHandler(OnMouseMove),
                    [Mouse.MouseUpEvent] = new MouseButtonEventHandler(OnMouseUp),
                    [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(OnMouseScroll)
                };

        private TilePloppinMode TilePloppinMode { get; set; }

        private float Scale {
            get { return scale_; }
            set
            {
                scale_ = MathHelper.Clamp(value, 0.3f, 500f);
            }
        }

        public AssetManagerView()
        {
            Blocks = new ObservableCollection<TileModel>();
            Platforms = new ObservableCollection<TileModel>();

            LoadCommand = new RelayCommand(OnLoad);
            DeleteCommand = new RelayCommand(OnDelete);
        }

        private void OnMouseDown(object source, MouseButtonEventArgs mouseEventArgs)
        {
            switch(mouseEventArgs.ChangedButton)
            {
                case MouseButton.Left:
                {
                    TilePloppinMode = TilePloppinMode.Ploppin;
                    break;
                }
                case MouseButton.Right:
                {
                    TilePloppinMode = TilePloppinMode.Deletin;
                    break;
                }
                default:
                {
                    /* 
                        Don't change ploppin' mode - this could be the case of something like a middle mouse after a left click 
                        (while left is still depressed). In this case, we want to ignore the extraneous event, and carry on our merry way 
                    */
                    Logger.Info("Ignoring additional mouse event for button: {0}", mouseEventArgs.ChangedButton);
                    break;
                }
            }
            HandleTilePloppin();
        }

        private void OnMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            // TODO: Punt to camera model?
            new ZoomRequest(Scale).Emit();
        }

        private void OnMouseMove(object source, MouseEventArgs mouseEventArgs)
        {
            HandleTilePloppin();
        }

        private void OnMouseUp(object source, MouseButtonEventArgs mouseEventArgs)
        {
            TilePloppinMode = TilePloppinMode.None;
        }

        private void HandleTilePloppin()
        {
            switch(TilePloppinMode)
            {
                case TilePloppinMode.None:
                {
                    // Ain't nothing to do, move along
                    return;
                }
                case TilePloppinMode.Deletin:
                {
                    new RemoveTileFromMapRequest().Emit();
                    return;
                }
                case TilePloppinMode.Ploppin:
                {
                    new AddTileToMapRequest().Emit();
                    return;
                }
                default:
                {
                    throw new InvalidEnumArgumentException($"Unknown {typeof(TilePloppinMode)}: {TilePloppinMode}");
                }
            }
        }

        private void OnLoad(object eventArgs)
        {
            OpenFileDialog loadAssetDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Filter = "Image files(*.png;*.jpg)|*.png;*.jpg;",
                Multiselect = false // TODO: True, handle multi files below
            };

            switch(loadAssetDialog.ShowDialog())
            {
                case DialogResult.OK:
                {
                    // TODO: Check that DialogResult is within Content directory, alert if not
                    string imagePath = loadAssetDialog.FileName;
                    Uri fullPath = new Uri(imagePath);
                    Uri contentDirectory =
                        new Uri(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar +
                                DxGame.Instance.Content.RootDirectory + Path.DirectorySeparatorChar);

                    if(!contentDirectory.IsBaseOf(fullPath))
                    {
                        // TODO: Make this *any* content directory, not just our own. Our own is kinda annoying.
                        string message =
                            $"{imagePath} is an invalid asset. Assets must be within the Content Directory: {DxGame.Instance.Content.RootDirectory}";
                        Logger.Error(message);
                        IMessageBoxService messageBoxService = GetService<IMessageBoxService>();
                        if(ReferenceEquals(messageBoxService, null))
                        {
                            return;
                        }
                        RelayCommand doNothing = new RelayCommand(arbitrary => { });
                        messageBoxService.Show(message, doNothing, false);
                        return;
                    }
                    try
                    {
                        using(Stream fileStream = loadAssetDialog.OpenFile())
                        {
                            Uri relative = contentDirectory.MakeRelativeUri(fullPath);
                            Texture2D imageAsTexture = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, fileStream);
                            imageAsTexture.Name = relative.ToString();
                            Blocks.Add(new TileModel(imageAsTexture));
                            Logger.Debug("Loaded {0}", imagePath);
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Error(e, "Failed to load {0}", imagePath);
                    }
                    break;
                }
                // TODO: Don't care
            }

            Console.WriteLine($"Load called, waow. With: {eventArgs}");
        }

        private void OnDelete(object eventArgs)
        {
            Console.WriteLine($"Delete called, nice! With: {eventArgs}");
        }
    }
}
