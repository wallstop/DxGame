using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Extension;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Media.Imaging;
using EmptyKeys.UserInterface.Mvvm;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Models
{
    public class FrameModel : BindableBase
    {
        public BitmapImage Frame { get; }

        public FrameModel(Texture2D texture, DxRectangle bounds)
        {
            Validate.Hard.IsNotNull(texture, () => this.GetFormattedNullOrDefaultMessage(texture));

            Texture2D cropped = texture.Crop(bounds);
            Frame = new BitmapImage {Texture = new MonoGameTexture(cropped)};
        }
    }
}