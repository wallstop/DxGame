﻿using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Services
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

    public sealed class ExperienceService : DxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Player, int> entitiesContributingToExperienceByPlayer_;

        public ExperienceFunction ExperienceFunction { get; }

        public ExperienceService() : this(SimpleExponentialEaseInOut) {}

        public ExperienceService(ExperienceFunction experienceFunction)
        {
            Validate.Hard.IsNotNullOrDefault(experienceFunction,
                () => this.GetFormattedNullOrDefaultMessage(experienceFunction));
            entitiesContributingToExperienceByPlayer_ = new Dictionary<Player, int>();
            ExperienceFunction = experienceFunction;
        }

        protected override void OnCreate()
        {
            Self.MessageHandler.RegisterMessageHandler<ExperienceDroppedMessage>(HandleExperienceDroppedMessage);
            Self.MessageHandler.RegisterMessageHandler<NewPlayerNotification>(HandleNewPlayer);
        }

        private void HandleExperienceDroppedMessage(ExperienceDroppedMessage experienceDropped)
        {
            /* 
                We could do some smart spatial tree style things, but we know there are 
                only ever going to be (some small number of players), so we may as well do 
                direct, simple distance compares
            */
            foreach(KeyValuePair<Player, int> playerAndEntityCount in entitiesContributingToExperienceByPlayer_.ToList()
            )
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

        private void HandleNewPlayer(NewPlayerNotification newPlayerNotification)
        {
            Player player = newPlayerNotification.Player;
            if(entitiesContributingToExperienceByPlayer_.ContainsKey(player))
            {
                Logger.Warn("Received double registration of {0}: {1}", typeof(Player), player);
                return;
            }
            entitiesContributingToExperienceByPlayer_[player] = 0;
        }

        private static float SimpleExponentialEaseInOut(int x)
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
    }
}