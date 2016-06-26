using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using DxCore;
using DxCore.Core.Primitives;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace MapEditorLibrary.Controls
{
    public class AssetManagerView : BindableBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ICommand loadCommand_;
        private ICommand deleteCommand_;
        private ICommand placeTileCommand_;
        private object selectedTile_;

        public ICommand LoadCommand
        {
            get { return loadCommand_; }
            set { SetProperty(ref loadCommand_, value); }
        }

        public ICommand DeleteCommand
        {
            get { return deleteCommand_; }
            set { SetProperty(ref deleteCommand_, value); }
        }

        public ICommand PlaceTileCommand
        {
         get { return placeTileCommand_;}
            set { SetProperty(ref placeTileCommand_, value); }   
        }

        public object SelectedTile
        {
            get { return selectedTile_; }
            set { SetProperty(ref selectedTile_, value); }
        }

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
            /*
                TODO:
                Get mouse coordinates.
                Translate to DxGame worldspace.
                Emit TilePlacement message.
            */

            // TODO: Rip this out, I'd rather rely on UI mouse position
            DxVector2 mousePosition = Mouse.GetState().Position;

            Console.WriteLine($"Tile placed, or at least it would if that was implemented. That's pretty cool. {mousePosition}");
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
                    string imagePath = loadAssetDialog.FileName;
                    try
                    {
                        using(Stream fileStream = loadAssetDialog.OpenFile())
                        {
                            Texture2D imageAsTexture = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, fileStream);
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

            // loadAssetDialog.OpenFile()
            Console.WriteLine($"Load called, waow. With: {eventArgs}");
        }

        private void OnDelete(object eventArgs)
        {
            Console.WriteLine($"Delete called, nice! With: {eventArgs}");
        }
    }
}
