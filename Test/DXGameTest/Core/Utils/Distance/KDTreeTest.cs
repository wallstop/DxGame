using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Distance;
using NUnit.Framework;

namespace DXGameTest.Core.Utils.Distance
{
    public class KDTreeTest
    {
        [Test]
        public void TestInRange()
        {
            int numPoints = 1000;
            DxRectangle range = new DxRectangle(200, 250, 10000, 10000);
            List<DxVector2> points = DistanceHelper.GeneratePointsInRange(range, numPoints);

            List<TestPoint> testObjects = points.Select(point => new TestPoint(point)).ToList();
            DxRectangle selectRange = range;

            KDTree<TestPoint> testTree = new KDTree<TestPoint>(testObject => testObject.Point, range, testObjects);
            List<TestPoint> objectsInRange = testTree.InRange(selectRange);

            Assert.AreEqual(testObjects.Count, objectsInRange.Count);
        }
    }
}
