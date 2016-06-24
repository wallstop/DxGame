using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditorLibrary.Controls
{
    public class TileModel : BindableBase
    {
        public BitmapImage Tile { get; }

        public Texture2D Texture { get; }

        public TileModel(Texture2D texture)
        {
            Validate.Hard.IsNotNull(texture);
            Texture = texture;
            Tile = new BitmapImage {Texture = new MonoGameTexture(texture)};
        }
    }
}
