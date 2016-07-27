using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Components.Developer;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Services.Components
{
    public sealed class DeveloperComponentHandler : DrawableComponent
    {
        private GameElementCollection DeveloperComponents { get; }
        private DeveloperSwitch DevSwitch { get; }

        public DeveloperComponentHandler(GameElementCollection gameElements, DeveloperSwitch devSwitch)
        {
            DrawPriority = DrawPriority.HudLayer;
            Validate.Hard.IsNotNull(gameElements);
            DeveloperComponents = gameElements;
            Validate.Hard.IsNotNull(devSwitch);
            DevSwitch = devSwitch;
        }

        public override void Initialize()
        {
            foreach(var component in from object element in DeveloperComponents select element as Component)
            {
                component?.Initialize();
            }
            DevSwitch.Initialize();
            base.Initialize();
        }

        public override void LoadContent()
        {
            foreach(var component in from object element in DeveloperComponents select element as Component)
            {
                component?.LoadContent();
            }
            DevSwitch.LoadContent();
            base.LoadContent();
        }

        protected override void Update(DxGameTime gameTime)
        {
            DevSwitch.Process(gameTime);
            if(DevSwitch.DeveloperMode != DeveloperMode.NotSoOn)
            {
                foreach(var updateable in DeveloperComponents.Processables)
                {
                    updateable.Process(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(DevSwitch.DeveloperMode != DeveloperMode.NotSoOn)
            {
                foreach(var drawable in DeveloperComponents.Drawables)
                {
                    drawable.Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
