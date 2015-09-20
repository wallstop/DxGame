using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        TODO: Wire this into a player's abilities. This should end up involving a Factory that creates these bad boys
    */
    [Serializable]
    [DataContract]
    public class AbilityBar : HudComponent
    {
        // Gap between each Skill area
        private static readonly int MIN_PIXEL_GAP = 5;

        //private static readonly int NUM_SKILLS = 4;

        private AbilityBar(DxGame game)
            : base(game)
        {
        }

        private IEnumerable<Rectangle> SkillAreas(int numSkills)
        {
            Validate.IsTrue(numSkills >= 0, $"Cannot generate a skill area for {numSkills} skills");
            /*  
                We want n skills, that means we need n + 1 gaps (one at the start, 
                one in between every skill, and one at the end (1 + n - 1 + 1) -> n + 1 
            */
            int numHorizontalDivisions = 2;
            int numVerticalDivisions = numSkills + 1;

            DxRectangle hudRegion = DxGame.GameSettings.HudRegion;
            // TODO: Come up with a better splitting algorithm. There's gotta be one publicly available somewhere
            float availableWidth = hudRegion.Width - (numVerticalDivisions * MIN_PIXEL_GAP);
            // SkillAreas are square, so same width & height
            float skillAreaWidth = Math.Min(availableWidth / numSkills,
                hudRegion.Height - (numHorizontalDivisions * MIN_PIXEL_GAP));
            float horizontalGap = hudRegion.Width - (skillAreaWidth * numSkills);
            float verticalGap = (hudRegion.Height - skillAreaWidth) / numHorizontalDivisions;
            List<Rectangle> skillAreas = new List<Rectangle>(numSkills);

            // We lay them out horizontally, so they all have the same y coordinate
            int y = (int)(hudRegion.Y + verticalGap);
            for (int i = 0; i < numSkills; ++i)
            {
                float x = hudRegion.X + ((i + 1) * horizontalGap) + (i * skillAreaWidth);

                DxRectangle skillArea = new DxRectangle(x, y, skillAreaWidth, skillAreaWidth);
                skillAreas.Add(skillArea.ToRectangle());
            }
            return skillAreas;
        }

        public class AbilityBarBuilder : IBuilder<AbilityBar>
        {
            //private Dictionary<int, > 


            public AbilityBar Build()
            {
                throw new NotImplementedException();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
    }
}