using DXGame.Main;

namespace DXGame.Core.Components.Advanced
{
    public class PlayerProperties : PropertiesComponent
    {
        public int Health { get; set; }

        public PlayerProperties(DxGame game)
            : base(game)
        {
        }
    }
}