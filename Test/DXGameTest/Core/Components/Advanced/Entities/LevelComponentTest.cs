using DXGame.Core;
using DXGame.Core.Components.Advanced.Entities;
using DXGame.Core.Experience;
using DXGame.Core.Messaging;
using NUnit.Framework;

namespace DXGameTest.Core.Components.Advanced.Entities
{
    public class LevelComponentTest
    {
        [Test]
        public void TestFibonacciLevelSystem()
        {
            Assert.AreEqual(100, LevelComponent.DetermineExperienceForNextLeveL(0));
            Assert.AreEqual(200, LevelComponent.DetermineExperienceForNextLeveL(1));
            Assert.AreEqual(300, LevelComponent.DetermineExperienceForNextLeveL(2));
            Assert.AreEqual(500, LevelComponent.DetermineExperienceForNextLeveL(3));
            Assert.AreEqual(800, LevelComponent.DetermineExperienceForNextLeveL(4));
            Assert.AreEqual(1300, LevelComponent.DetermineExperienceForNextLeveL(5));
            Assert.AreEqual(2100, LevelComponent.DetermineExperienceForNextLeveL(6));
            Assert.AreEqual(3400, LevelComponent.DetermineExperienceForNextLeveL(7));
            Assert.AreEqual(5500, LevelComponent.DetermineExperienceForNextLeveL(8));
            Assert.AreEqual(8900, LevelComponent.DetermineExperienceForNextLeveL(9));
            Assert.AreEqual(14400, LevelComponent.DetermineExperienceForNextLeveL(10));
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
            levelComponent.MessageHandler.HandleMessage(experiencedReceived);
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