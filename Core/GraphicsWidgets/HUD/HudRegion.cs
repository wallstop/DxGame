using System;
using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public class HudRegion : DrawableComponent
    {
        public HudRegion(DxGame game)
            : base(game)
        {
        }

        public IEnumerable<Rectangle> SkillAreas(int numSkills)
        {
            Validate.IsTrue(numSkills >= 0, $"Cannot generate a skill area for {numSkills} skills");
            return null;
        }

        public override void Draw(DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}