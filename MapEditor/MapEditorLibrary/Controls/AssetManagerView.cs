using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using DxCore;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace MapEditorLibrary.Controls
{
    public class AssetManagerView : BindableBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ICommand loadCommand_;
        private ICommand deleteCommand_;

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

        public ObservableCollection<TileModel> Blocks { get; }
        public ObservableCollection<TileModel> Platforms { get; }

        public AssetManagerView()
        {
            Blocks = new ObservableCollection<TileModel>();
            Platforms = new ObservableCollection<TileModel>();

            LoadCommand = new RelayCommand(OnLoad);
            DeleteCommand = new RelayCommand(OnDelete);
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
                    // TODO: Can read stream in and do whatever, this is probably wayyyy easier
                    string imagePath = loadAssetDialog.FileName;
                    try
                    {
                        using(Stream fileStream = loadAssetDialog.OpenFile())
                        {

                            Texture2D imageAsTexture = Texture2D.FromStream(DxGame.Instance.GraphicsDevice, fileStream);

                            BitmapImage tileImage = new BitmapImage {Texture = new MonoGameTexture(imageAsTexture)};

                            Blocks.Add(new TileModel(tileImage));
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
