using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DxCore.Core.Models
{
    /**
        <summary>
            Answers the question "How much should I scale an experience value by?" 
            based on number of entities that have already contributed to experience growth.
            This function should return a value between (0, 1] and should be incorporated as:
            (Base Experience) * (function result) = (experience gained)
        </summary>
    */

    public delegate float ExperienceFunction(int numberOfEntitiesKilled);

    public class ExperienceModel : Model
    {
        private readonly Dictionary<Player, int> entitiesContributingToExperienceByPlayer_;

        public ExperienceFunction ExperienceFunction { get; }

        public ExperienceModel() : this(SimpleExponentialEaseInOut) {}

        public override bool ShouldSerialize => false;

        public ExperienceModel(ExperienceFunction experienceFunction)
        {
            Validate.IsNotNullOrDefault(experienceFunction,
                StringUtils.GetFormattedNullOrDefaultMessage(this, experienceFunction));
            entitiesContributingToExperienceByPlayer_ = new Dictionary<Player, int>();
            ExperienceFunction = experienceFunction;
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<ExperienceDroppedMessage>(HandleExperienceDroppedMessage);
            base.OnAttach();
        }

        protected static float SimpleExponentialEaseInOut(int x)
        {
            /* Just make use of our already-populated Spring Functions */
            const int maxEntities = 100;
            const float floor = 0.001f;
            if(x >= maxEntities)
            {
                return floor;
            }
            /* Start at 1 and go to 0 - earlier mob kills should weigh "more" */
            float scalar = (float) SpringFunctions.ExponentialEaseOutIn(1, 0, x, maxEntities);
            return scalar;
        }

        protected override void Update(DxGameTime gameTime)
        {
            /* Make sure we know about all of the players */
            PlayerModel playerModel = DxGame.Instance.Model<PlayerModel>();
            foreach(Player player in playerModel.Players)
            {
                if(entitiesContributingToExperienceByPlayer_.ContainsKey(player))
                {
                    continue;
                }
                entitiesContributingToExperienceByPlayer_[player] = 0;
            }

            base.Update(gameTime);
        }

        private void HandleExperienceDroppedMessage(ExperienceDroppedMessage experienceDropped)
        {
            /* 
                We could do some smart spatial tree style things, but we know there are 
                only ever going to be (some small number of players), so we may as well do 
                direct, simple distance compares
            */
            foreach(KeyValuePair<Player, int> playerAndEntityCount in entitiesContributingToExperienceByPlayer_.ToList())
            {
                Player player = playerAndEntityCount.Key;
                DxVector2 playerPosition = player.Position.Center;
                float distanceBetweenPlayerAndXp = (playerPosition - experienceDropped.Position).Magnitude;
                if(distanceBetweenPlayerAndXp > experienceDropped.Radius)
                {
                    continue;
                }
                int entityCount = playerAndEntityCount.Value;
                float scalar = ExperienceFunction(entityCount);
                int experienceValue = (int) Math.Round(scalar * experienceDropped.Experience.Value);
                ExperiencedReceivedMessage experienceReceived =
                    new ExperiencedReceivedMessage(new Experience.Experience(experienceValue));
                ++entitiesContributingToExperienceByPlayer_[player];
                experienceReceived.Target = player.Object.Id;
                experienceReceived.Emit();
            }
        }
    }
}