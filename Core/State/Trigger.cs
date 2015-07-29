using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.State
{
    /*
        Triggers serve as the gateways to states. Triggers should be of the form (has some condition been fulfilled?)
    */

    public delegate bool Trigger(DxGame game, DxGameTime gameTime);
}