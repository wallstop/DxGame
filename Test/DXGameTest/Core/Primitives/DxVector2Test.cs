using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Primitives
{
    public class DxVector2Test
    {
        private const double TOLERANCE = 1E-5;

        [Test]
        public void TestUnitVector()
        {
            int numTestIterations = 100000;
            for (int i = 0; i < numTestIterations; ++i)
            {
                var generatedVector = GenerateRandomVector();
                var unitVector = generatedVector.UnitVector;
                Assert.AreEqual(1, unitVector.MagnitudeSquared, TOLERANCE);
                Assert.AreEqual(1, unitVector.Magnitude, TOLERANCE);
            }
        }

        private static DxVector2 GenerateRandomVector()
        {
            return new DxVector2(ThreadLocalRandom.Current.NextFloat(int.MinValue, int.MaxValue), ThreadLocalRandom.Current.NextFloat(int.MinValue, int.MaxValue));
        }
    }
}
