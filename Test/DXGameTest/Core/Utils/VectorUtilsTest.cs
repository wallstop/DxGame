using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Utils
{
    [TestFixture]
    public class VectorUtilsTest
    {
        [Test]
        public void TestConstainVectorXViolation()
        {
            float min = -25.0f;
            float max = 100.0f;
            var testVector = new Vector2(-100.0f, max);

            Vector2 resultVector = VectorUtils.ClampVector(testVector, min, max);
            // Ensure that the resultVector has been properly constrained
            Assert.AreNotEqual(testVector, resultVector);
            Assert.AreEqual(testVector.Y, resultVector.Y);
            // Only the X value should have changed
            Assert.Less(testVector.X, resultVector.X);
            Assert.AreEqual(min, resultVector.X);
        }

        [Test]
        public void TestConstrainVectorBadMinMax()
        {
            float min = 100.0f;
            float max = -100.0f;
            var testVector = new Vector2(min - 100.0f, max + 100.0f);
        }

        [Test]
        public void TestConstrainVectorBothViolation()
        {
            float min = -100.0f;
            float max = 100.0f;
            var testVector = new Vector2(min - 100.0f, max + 100.0f);

            Vector2 resultVector = VectorUtils.ClampVector(testVector, min, max);
            // Ensure that the resultVector has been properly constrained
            Assert.AreNotEqual(testVector, resultVector);
            Assert.AreEqual(min, resultVector.X);
            Assert.AreEqual(max, resultVector.Y);
        }

        [Test]
        public void TestConstrainVectorNoViolation()
        {
            var testVector = new Vector2(1.0f, 1.0f);
            float min = float.MinValue;
            float max = float.MaxValue;

            Vector2 resultVector = VectorUtils.ClampVector(testVector, min, max);
            // Ensure that the returned vector remains unchanged
            Assert.AreEqual(testVector, resultVector);
        }

        [Test]
        public void TestConstrainVectorYViolation()
        {
            float min = -25.0f;
            float max = 100.0f;
            var testVector = new Vector2(2.0f, max + 300.0f);

            Vector2 resultVector = VectorUtils.ClampVector(testVector, min, max);
            // Ensure that the resultVector has been properly constrained
            Assert.AreNotEqual(testVector, resultVector);
            Assert.AreEqual(testVector.X, resultVector.X);
            // Only the Y value should have changed
            Assert.Greater(testVector.Y, resultVector.Y);
            Assert.AreEqual(max, resultVector.Y);
        }

        [Test]
        public void TestConstrianVectorWithVector()
        {
            Vector2 constraint = new Vector2(2.0f, 15.0f);

            Vector2 constrained = VectorUtils.ClampVector(new Vector2(3, -14), constraint);
            Assert.AreNotEqual(constraint, constrained);
            Assert.AreEqual(new Vector2(2, -14), constrained);
        }
    }
}