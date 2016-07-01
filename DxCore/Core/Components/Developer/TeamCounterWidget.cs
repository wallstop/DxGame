using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Advanced;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary>
            Counts and Draws (in the upper left) the number of current Components that belong to each team
        </summary>
    */

    public class TeamCounterWidget : DrawableComponent
    {
        private static readonly DxVector2 OFFSET = new DxVector2(5, 5);
        private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromSeconds(1.0);
        protected readonly Dictionary<Team, int> teamCounts_ = new Dictionary<Team, int>();
        private TimeSpan lastUpdated_ = TimeSpan.Zero;
        protected SpriteFont spriteFont_;

        public override void LoadContent()
        {
            spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles");
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* Only count so often */
            if (lastUpdated_ + UPDATE_INTERVAL >= gameTime.TotalGameTime)
            {
                return;
            }

            teamCounts_.Clear();
            /* Grab all team components */
            var teamComponents = DxGame.Instance.DxGameElements.OfType<TeamComponent>();
            foreach (var team in teamComponents.Select(teamComponent => teamComponent.Team))
            {
                int currentCount;
                /* currentValue will come out as default value if there are no entries for the key */
                teamCounts_.TryGetValue(team, out currentCount);
                ++currentCount;
                teamCounts_[team] = currentCount;
            }
            lastUpdated_ = gameTime.TotalGameTime;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // No teams? Nothing to do.
            if (!teamCounts_.Any())
            {
                return;
            }
            var teamCountStrings =
                teamCounts_.Select(teamAndCount => $"{teamAndCount.Key}: {teamAndCount.Value}").ToList();
            /* Sort them largest first */
            List<string> teamCountsSortedByLength = teamCountStrings.ToList();
            teamCountsSortedByLength.Sort(Comparer<string>.Create((first, second) => second.Length - first.Length));

            var size = spriteFont_.MeasureString(teamCountsSortedByLength[0]);
            Vector2 drawLocation = DxGame.Instance.OffsetFromScreen(OFFSET);

            var textHeight = size.Y;
            var drawSize = new Vector2(size.X, size.Y * teamCountsSortedByLength.Count);
            var blackTexture = TextureFactory.TextureForColor(Color.Black);
            const float transparencyWeight = 0.8f;
            var transparency = ColorFactory.Transparency(transparencyWeight);
            /* Draw a neato transparent box behind the text to make the text "pop" */
            spriteBatch.Draw(blackTexture, color: transparency,
                destinationRectangle:
                    new Rectangle((int) drawLocation.X, (int) drawLocation.Y, (int) drawSize.X, (int) drawSize.Y));
            for (int i = 0; i < teamCountsSortedByLength.Count; ++i)
            {
                spriteBatch.DrawString(spriteFont_, teamCountsSortedByLength[i],
                    new Vector2(drawLocation.X, drawLocation.Y + textHeight * i), Color.DarkOrange);
            }
        }
    }
}