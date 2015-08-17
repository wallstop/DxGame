using System;
using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
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

            DxRectangle selectRange = rectangles[0];

            Stopwatch timer = Stopwatch.StartNew();

            RTree<DxRectangle> testTree = new RTree<DxRectangle>(rectangle => rectangle, rectangles);
            TimeSpan treeBuildTime = timer.Elapsed;
            List<DxRectangle> objectsInRange = testTree.InRange(selectRange);
            timer.Stop();

            Console.WriteLine(
                $"RTree build time {treeBuildTime}, full query time {timer.Elapsed - treeBuildTime}, total {timer.Elapsed}");

            Assert.AreEqual(rectangles.Count, objectsInRange.Count);
        }
    }
}
