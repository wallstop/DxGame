using System;
using System.Collections.Generic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public class HudRegion : HudComponent
    {
        // Gap between each Skill area
        private static readonly int MIN_PIXEL_GAP = 5;
        /* 
            The delay for which we check to see whether or not we have a different number of skills (since our last check)
        */
        private static readonly TimeSpan SKILL_CHECK_DELAY = TimeSpan.FromSeconds(1.0);
        //private IEnumerable<Rectangle> skillArea_;
        private TimeSpan skillAreaLastUpdated_ = TimeSpan.FromSeconds(0.0);

        public HudRegion(DxGame game)
            : base(game)
        {
        }

        public IEnumerable<Rectangle> SkillAreas(int numSkills)
        {
            Validate.IsTrue(numSkills >= 0, $"Cannot generate a skill area for {numSkills} skills");
            /*  
                We want n skills, that means we need n + 1 gaps (one at the start, 
                one in between every skill, and one at the end (1 + n - 1 + 1) -> n + 1 
            */
            int numHorizontalDivisions = numSkills + 1;
            int numVerticalDivisions = 2;

            DxRectangle hudRegion = DxGame.GameSettings.HudRegion;
            // TODO: Come up with a better splitting algorithm. There's gotta be one publicly available somewhere
            float availableWidth = hudRegion.Width - (numHorizontalDivisions * MIN_PIXEL_GAP);
            // SkillAreas are square, so same width & height
            float skillAreaWidth = Math.Min(availableWidth / numSkills,
                hudRegion.Height - (numVerticalDivisions * MIN_PIXEL_GAP));
            float horizontalGap = hudRegion.Width - (skillAreaWidth * numSkills);
            float verticalGap = (hudRegion.Height - skillAreaWidth) / numVerticalDivisions;
            List<Rectangle> skillAreas = new List<Rectangle>(numSkills);

            // We lay them out horizontally, so they all have the same y coordinate
            int y = (int) (hudRegion.Y + verticalGap);
            for (int i = 0; i < numSkills; ++i)
            {
                float x = hudRegion.X + ((i + 1) * horizontalGap) + (i * skillAreaWidth);

                DxRectangle skillArea = new DxRectangle(x, y, skillAreaWidth, skillAreaWidth);
                skillAreas.Add(skillArea.ToRectangle());
            }
            return skillAreas;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var totalTime = gameTime.TotalGameTime;
            if (totalTime >= skillAreaLastUpdated_.Add(SKILL_CHECK_DELAY))
            {
                // TODO
            }

            throw new NotImplementedException();
        }
    }
}