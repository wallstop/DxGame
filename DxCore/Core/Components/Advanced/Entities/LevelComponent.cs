using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DXGame.Core.Messaging;
using NLog;

namespace DxCore.Core.Components.Advanced.Entities
{
    [DataContract]
    [Serializable]
    public class LevelComponent : Component
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public static readonly int BASE_EXPERIENCE_TO_LEVEL = 100;

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
            Level = 0;
            CurrentExperience = 0;
            ExperienceToLevel = DetermineExperienceForNextLevel(Level);
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<ExperiencedReceivedMessage>(HandleExperienceReceivedMessage);
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
                ExperienceToLevel = DetermineExperienceForNextLevel(Level);
                LeveledUpMessage leveledUp = new LeveledUpMessage(Parent, Level);
                leveledUp.Emit();
            }
        }

        public static int DetermineExperienceForNextLevel(int currentLevel)
        {
            /*
                https://en.wikipedia.org/wiki/Fibonacci_number#Relation_to_the_golden_ratio
                Closed-form enough ish
            */
            const int offset = 2;
            /* 
                FrameOffset the level by 2 - the following formula is assuming the following fibonacci mapping:
                    0: 0
                    1: 1
                    2: 1
                    3: 2
                    4: 3
                    ...
                So we want to actually start at the second fibonacci number, but our level starts out 0, so we need the offset
            */
            currentLevel += offset;
            double sqrtFive = Math.Sqrt(5);
            double alpha = (1 + sqrtFive) / 2;
            double theta = (1 - sqrtFive) / 2;

            return
                (int)
                    Math.Round((Math.Pow(alpha, currentLevel) - Math.Pow(theta, currentLevel)) / sqrtFive *
                               BASE_EXPERIENCE_TO_LEVEL);
        }
    }
}