using DxCore.Core;
using DxCore.Core.Components.Advanced.Entities;
using DxCore.Core.Experience;
using DxCore.Core.Messaging;
using NUnit.Framework;

namespace DXGameTest.Core.Components.Advanced.Entities
{
    public class LevelComponentTest
    {
        [Test]
        public void TestFibonacciLevelSystem()
        {
            Assert.AreEqual(100, LevelComponent.DetermineExperienceForNextLevel(0));
            Assert.AreEqual(200, LevelComponent.DetermineExperienceForNextLevel(1));
            Assert.AreEqual(300, LevelComponent.DetermineExperienceForNextLevel(2));
            Assert.AreEqual(500, LevelComponent.DetermineExperienceForNextLevel(3));
            Assert.AreEqual(800, LevelComponent.DetermineExperienceForNextLevel(4));
            Assert.AreEqual(1300, LevelComponent.DetermineExperienceForNextLevel(5));
            Assert.AreEqual(2100, LevelComponent.DetermineExperienceForNextLevel(6));
            Assert.AreEqual(3400, LevelComponent.DetermineExperienceForNextLevel(7));
            Assert.AreEqual(5500, LevelComponent.DetermineExperienceForNextLevel(8));
            Assert.AreEqual(8900, LevelComponent.DetermineExperienceForNextLevel(9));
            Assert.AreEqual(14400, LevelComponent.DetermineExperienceForNextLevel(10));
        }

        [Test]
        public void TestMultipleLevelUpFromOneExperienceMessage()
        {
            GameObject playerPlaceholder = GameObject.Builder().Build();
            LevelComponent levelComponent = new LevelComponent();
            playerPlaceholder.AttachComponent(levelComponent);
            int ludicrousExperience = 1000000;
            ExperiencedReceivedMessage experiencedReceived =
                new ExperiencedReceivedMessage(new Experience(ludicrousExperience));

            playerPlaceholder.MessageHandler.ProcessTypedMessage<ExperiencedReceivedMessage>(experiencedReceived);
            /* We expect that the player should have leveled up more than once (how many times, we don't really care) */
            Assert.IsTrue(levelComponent.Level > 1);
        }

        [Test]
        public void TestLevelComponentStartsOutZerod()
        {
            LevelComponent levelComponent = new LevelComponent();
            Assert.AreEqual(0, levelComponent.Level);
            Assert.AreEqual(100, levelComponent.ExperienceToLevel);
            Assert.AreEqual(0, levelComponent.CurrentExperience);
        }
    }
}