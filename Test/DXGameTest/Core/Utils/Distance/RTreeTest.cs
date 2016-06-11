using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Distance;
using NUnit.Framework;

namespace DXGameTest.Core.Utils.Distance
{
    public class RTreeTest
    {
        [Test]
        public void TestInRange()
        {
            int numPoints = 1000;
            DxRectangle range = new DxRectangle(200, 250, 10000, 10000);
            List<DxRectangle> rectangles = DistanceHelper.GenerateRectanglesInRange(range, numPoints);

            DxRectangle selectRange = range;
            RTree<DxRectangle> testTree = new RTree<DxRectangle>(rectangle => rectangle, rectangles);
            List<DxRectangle> objectsInRange = testTree.InRange(selectRange);

            Assert.AreEqual(rectangles.Count, objectsInRange.Count);
        }
    }
}
