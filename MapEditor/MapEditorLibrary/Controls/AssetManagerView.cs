using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using DxCore;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using MapEditorLibrary.Core.Messaging;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace MapEditorLibrary.Controls
{
    public class AssetManagerView : ViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private object selectedTile_;

        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand PlaceTileCommand { get; }

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

        public AssetManagerView()
        {
            Blocks = new ObservableCollection<TileModel>();
            Platforms = new ObservableCollection<TileModel>();

            LoadCommand = new RelayCommand(OnLoad);
            DeleteCommand = new RelayCommand(OnDelete);
            PlaceTileCommand = new RelayCommand(OnTilePlacement);
        }

        private void OnTilePlacement(object eventArgs)
        {
            new AddTileToMapRequest().Emit();
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
