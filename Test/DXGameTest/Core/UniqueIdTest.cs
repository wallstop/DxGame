using DXGame.Core;
using NUnit.Framework;

namespace DXGameTest.Core
{
    public class UniqueIdTest
    {
        [Test]
        public void TestUniqueId()
        {
            UniqueId firstId = new UniqueId();
            UniqueId copyOfFirst = new UniqueId(firstId);

            UniqueId secondId = new UniqueId();

            Assert.IsTrue(firstId.IsValid());
            Assert.IsTrue(secondId.IsValid());
            Assert.IsTrue(copyOfFirst.IsValid());

            Assert.AreNotEqual(firstId, secondId);
            Assert.Greater(firstId, secondId);
            Assert.AreEqual(firstId, copyOfFirst);
            Assert.AreEqual(0, firstId.CompareTo(copyOfFirst));
            Assert.AreEqual(1, firstId.CompareTo(secondId));
            Assert.AreEqual(0, copyOfFirst.CompareTo(firstId));
            Assert.AreEqual(1, copyOfFirst.CompareTo(secondId));
            Assert.AreEqual(-1, secondId.CompareTo(firstId));
            Assert.AreEqual(-1, secondId.CompareTo(copyOfFirst));
        }
    }
}