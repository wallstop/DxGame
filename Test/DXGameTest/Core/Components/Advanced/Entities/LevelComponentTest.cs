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
            Assert.AreEqual(100, EntityLevelComponent.DetermineExperienceForNextLevel(0));
            Assert.AreEqual(200, EntityLevelComponent.DetermineExperienceForNextLevel(1));
            Assert.AreEqual(300, EntityLevelComponent.DetermineExperienceForNextLevel(2));
            Assert.AreEqual(500, EntityLevelComponent.DetermineExperienceForNextLevel(3));
            Assert.AreEqual(800, EntityLevelComponent.DetermineExperienceForNextLevel(4));
            Assert.AreEqual(1300, EntityLevelComponent.DetermineExperienceForNextLevel(5));
            Assert.AreEqual(2100, EntityLevelComponent.DetermineExperienceForNextLevel(6));
            Assert.AreEqual(3400, EntityLevelComponent.DetermineExperienceForNextLevel(7));
            Assert.AreEqual(5500, EntityLevelComponent.DetermineExperienceForNextLevel(8));
            Assert.AreEqual(8900, EntityLevelComponent.DetermineExperienceForNextLevel(9));
            Assert.AreEqual(14400, EntityLevelComponent.DetermineExperienceForNextLevel(10));
        }

        [Test]
        public void TestMultipleLevelUpFromOneExperienceMessage()
        {
            GameObject playerPlaceholder = GameObject.Builder().Build();
            EntityLevelComponent entityLevelComponent = new EntityLevelComponent();
            playerPlaceholder.AttachComponent(entityLevelComponent);
            int ludicrousExperience = 1000000;
            ExperiencedReceivedMessage experiencedReceived =
                new ExperiencedReceivedMessage(new Experience(ludicrousExperience));

            playerPlaceholder.MessageHandler.HandleTypedMessage<ExperiencedReceivedMessage>(experiencedReceived);
            /* We expect that the player should have leveled up more than once (how many times, we don't really care) */
            Assert.IsTrue(entityLevelComponent.Level > 1);
        }

        [Test]
        public void TestLevelComponentStartsOutZerod()
        {
            EntityLevelComponent entityLevelComponent = new EntityLevelComponent();
            Assert.AreEqual(0, entityLevelComponent.Level);
            Assert.AreEqual(100, entityLevelComponent.ExperienceToLevel);
            Assert.AreEqual(0, entityLevelComponent.CurrentExperience);
        }
    }
}