using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;

namespace MapEditorLibrary.Controls
{
    public class TileModel : BindableBase
    {
        public BitmapImage Tile { get; }

        public TileModel(BitmapImage tile)
        {
            Validate.Hard.IsNotNull(tile);
            Tile = tile;
        }
    }
}
