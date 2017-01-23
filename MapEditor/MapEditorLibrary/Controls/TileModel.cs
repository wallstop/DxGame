using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace MapEditorLibrary.Controls
{
    public class TileModel : BindableBase
    {
        public Texture2D Texture { get; }
        public BitmapImage Tile { get; }

        public TileModel(Texture2D texture)
        {
            Validate.Hard.IsNotNull(texture);
            Texture = texture;
            Tile = new BitmapImage {Texture = new MonoGameTexture(texture)};
        }
    }
}