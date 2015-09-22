using System;
using DXGame.Core.Primitives;
using NUnit.Framework;

namespace DXGameTest.Core.Primitives
{
    public class DxRadianTest
    {
        private static readonly float TOLERANCE = 0.005f;

        [Test]
        public void TestUnitVectors()
        {
            var positiveXVector = new DxRadian((float) (Math.PI / 2)).UnitVector;
            var expectedPositiveXVector = new DxVector2(1, 0);
            Assert.AreEqual(expectedPositiveXVector.X, positiveXVector.X, TOLERANCE);
            Assert.AreEqual(expectedPositiveXVector.Y, positiveXVector.Y, TOLERANCE);

            var negativeXVector = new DxRadian((float) (3 * Math.PI / 2)).UnitVector;
            var expectedNegativeXVector = new DxVector2(-1, 0);
            Assert.AreEqual(expectedNegativeXVector.X, negativeXVector.X, TOLERANCE);
            Assert.AreEqual(expectedNegativeXVector.Y, negativeXVector.Y, TOLERANCE);

            var positiveYVector = new DxRadian((float) (Math.PI)).UnitVector;
            var expectedPositiveYVector = new DxVector2(0, 1);
            Assert.AreEqual(expectedPositiveYVector.X, positiveYVector.X, TOLERANCE);
            Assert.AreEqual(expectedPositiveYVector.Y, positiveYVector.Y, TOLERANCE);

            /* We expect -1 y to be "up", in terms of XNA game window */
            var negativeYVector = new DxRadian(0).UnitVector;
            var expectedNegativeYVector = new DxVector2(0, -1);
            Assert.AreEqual(expectedNegativeYVector.X, negativeYVector.X, TOLERANCE);
            Assert.AreEqual(expectedNegativeYVector.Y, negativeYVector.Y, TOLERANCE);
        }
    }
}