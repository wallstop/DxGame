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

    // TODO: Break up into modules or some shit
    public class AssetManagerView : ViewModelBase
    {
        private const float BaseScrollScale = 1200f;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly MapLayout DefaultMapLayout =
            MapLayout.Builder().WithTileHeight(50).WithTileWidth(50).WithHeight(100).WithWidth(100).Build();

        private int mapHeight_;
        private int mapWidth_;
        private float scale_ = 1.0f;

        private object selectedTile_;

        private int tileSize_;

        public ObservableCollection<TileModel> Blocks { get; }
        public ICommand DeleteTileCommand { get; }

        public Dictionary<RoutedEvent, Delegate> Handlers
            =>
            new Dictionary<RoutedEvent, Delegate>
            {
                [Mouse.MouseDownEvent] = new MouseButtonEventHandler(HandleMouseDown),
                [Mouse.MouseMoveEvent] = new MouseEventHandler(HandleMouseMove),
                [Mouse.MouseUpEvent] = new MouseButtonEventHandler(HandleMouseUp),
                [Mouse.MouseWheelEvent] = new MouseWheelEventHandler(HandleMouseScroll)
            };

        public ICommand LoadBlockCommand { get; }
        public ICommand LoadMapCommand { get; }

        public ICommand LoadPlatformCommand { get; }

        public int MapHeight
        {
            get { return mapHeight_; }
            set
            {
                SetProperty(ref mapHeight_, value);
                NofityMapLayoutChanged();
            }
        }

        public int MapWidth
        {
            get { return mapWidth_; }
            set
            {
                SetProperty(ref mapWidth_, value);
                NofityMapLayoutChanged();
            }
        }

        public ObservableCollection<TileModel> Platforms { get; }
        public ICommand ResetCommand { get; }
        public ICommand SaveMapCommand { get; }

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

        public TileType SelectedTileType { get; private set; } = TileType.None;

        public int TileSize
        {
            get { return tileSize_; }
            set
            {
                SetProperty(ref tileSize_, value);
                NofityMapLayoutChanged();
            }
        }

        private float Scale
        {
            get { return scale_; }
            set
            {
                /* TODO: Rip from CameraModel */
                scale_ = MathHelper.Clamp(value, 1 / 3f, 3f);
            }
        }

        private TilePloppinMode TilePloppinMode { get; set; }

        public AssetManagerView()
        {
            Blocks = new ObservableCollection<TileModel>();
            Platforms = new ObservableCollection<TileModel>();

            LoadBlockCommand = new RelayCommand(HandleBlockLoad);
            LoadPlatformCommand = new RelayCommand(HandlePlatformLoad);
            DeleteTileCommand = new RelayCommand(HandleTileDelete);
            LoadMapCommand = new RelayCommand(HandleMapLoad);
            SaveMapCommand = new RelayCommand(HandleMapSave);
            ResetCommand = new RelayCommand(HandleReset);

            tileSize_ = DefaultMapLayout.TileWidth;
            mapWidth_ = DefaultMapLayout.Width;
            mapHeight_ = DefaultMapLayout.Height;
            HandleReset(null);
        }

        private void HandleBlockLoad(object whoCares)
        {
            HandleTileLoad(Blocks);
        }

        private void HandleMapLoad(object whoCares)
        {
            OpenFileDialog loadMapDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,
                Filter = "MapDescriptor files(*.mdtr)|*.mdtr;",
                Multiselect = false
            };

            switch(loadMapDialog.ShowDialog())
            {
                case DialogResult.OK:
                {
                    string mapFilePath = loadMapDialog.FileName;
                    new LoadMapRequest(mapFilePath).Emit();
                    break;
                }
            }
        }

        private void HandleMapSave(object whoCares)
        {
            SaveFileDialog saveMapDialog = new SaveFileDialog
            {
                CheckPathExists = true,
                RestoreDirectory = true,
                Filter = "MapDescriptor files(*.mdtr)|*.mdtr;"
            };

            switch(saveMapDialog.ShowDialog())
            {
                case DialogResult.OK:
                {
                    string mapFilePath = saveMapDialog.FileName;
                    new SaveMapRequest(mapFilePath).Emit();
                    break;
                }
            }
        }

        private void HandleMouseDown(object source, MouseButtonEventArgs mouseEventArgs)
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

        private void HandleMouseMove(object source, MouseEventArgs mouseEventArgs)
        {
            HandleTilePloppin();
        }

        private void HandleMouseScroll(object source, MouseWheelEventArgs mouseEventArgs)
        {
            float delta = mouseEventArgs.Delta;
            delta /= BaseScrollScale;
            Scale += delta;
            // TODO: Punt to camera model?
            new ZoomRequest(Scale).Emit();
        }

        private void HandleMouseUp(object source, MouseButtonEventArgs mouseEventArgs)
        {
            TilePloppinMode = TilePloppinMode.None;
        }

        private void HandlePlatformLoad(object whoCares)
        {
            HandleTileLoad(Platforms);
        }

        private void HandleReset(object whoCares)
        {
            Blocks.Clear();
            Platforms.Clear();
            TileSize = DefaultMapLayout.TileWidth;
            MapWidth = DefaultMapLayout.Width;
            MapHeight = DefaultMapLayout.Height;
            new ResetMapRequest().Emit();
        }

        private void HandleTileDelete(object whoCares)
        {
            Console.WriteLine($"{nameof(HandleTileDelete)} called with some sweet args: {whoCares}. That's super rad.");
        }

        private void HandleTileLoad(Collection<TileModel> tileModels)
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
                    string imagePath = loadAssetDialog.FileName;
                    /* 
                        This is pretty shitty. Unfortunately, we aren't globally content-directory aware. So, this is a pretty quick & dirty mid-term fix.
                        Note: this doesn't work if we have some higher-level directory with the same name as our Content directory. Truly unfortunate. 
                        TODO: We could do something like manually, explicitly, exposing the option to select Content directories. That way we could get 
                        some strong gaurantees via the Uri class.
                    */
                    string contentRoot = DxGame.Instance.Content.RootDirectory;
                    if(!imagePath.Contains(contentRoot))
                    {
                        string message = $"{imagePath} is an invalid asset. Assets must be within the Content Directory";
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
                        /* +1 for line separator */
                        string relativePath =
                            imagePath.Substring(imagePath.IndexOf(contentRoot, StringComparison.Ordinal) +
                                                contentRoot.Length + 1);
                        using(Stream fileStream = loadAssetDialog.OpenFile())
                        {
                            Texture2D imageAsTexture = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, fileStream);
                            imageAsTexture.Name = relativePath;
                            tileModels.Add(new TileModel(imageAsTexture));
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

        private void NofityMapLayoutChanged()
        {
            MapLayout newLayout =
                MapLayout.Builder().WithTileSize(TileSize).WithWidth(MapWidth).WithHeight(MapHeight).Build();
            new MapLayoutChanged(newLayout).Emit();
        }
    }
}