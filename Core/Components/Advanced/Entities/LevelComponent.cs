using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Main;

namespace DXGame.Core.Components.Advanced.Entities
{
    [DataContract]
    [Serializable]
    public class LevelComponent : Component
    {
        private static readonly int BASE_EXPERIENCE_TO_LEVEL = 100;

        [DataMember]
        public int Level { get; private set; }

        [DataMember]
        public int ExperienceToLevel { get; private set; }

        [DataMember]
        public int CurrentExperience { get; private set; }

        [IgnoreDataMember]
        public float Progress => (float) CurrentExperience / ExperienceToLevel;

        public LevelComponent()
        {
            MessageHandler.RegisterMessageHandler<ExperiencedReceivedMessage>(HandleExperienceReceivedMessage);
        }

        private void HandleExperienceReceivedMessage(ExperiencedReceivedMessage experienceReceived)
        {
            int experience = experienceReceived.Experience.Value;
            CurrentExperience += experience;
            /* 
                Maybe we received some massive quantity of experience - 
                continue checking if we've exceeded our level until we haven't 
            */
            while(CurrentExperience >= ExperienceToLevel)
            {
                ++Level;
                CurrentExperience -= ExperienceToLevel;
                ExperienceToLevel = DetermineExperienceForNextLeveL(Level);
                PlayerLeveledUpMessage playerLeveledUp = new PlayerLeveledUpMessage(Parent, Level);
                DxGame.Instance.BroadcastMessage<PlayerLeveledUpMessage>(playerLeveledUp);
            }
        }

        private static int DetermineExperienceForNextLeveL(int currentLevel)
        {
            /*
                https://en.wikipedia.org/wiki/Fibonacci_number#Relation_to_the_golden_ratio
                Closed-form enough ish
            */
            double sqrtFive = Math.Sqrt(5);
            double alpha = (1 + sqrtFive) / 2;
            double theta = (1 - sqrtFive) / 2;

            return
                (int)
                    (Math.Round((Math.Pow(alpha, currentLevel) - Math.Pow(theta, currentLevel)) / sqrtFive *
                                BASE_EXPERIENCE_TO_LEVEL);
        }
    }
}