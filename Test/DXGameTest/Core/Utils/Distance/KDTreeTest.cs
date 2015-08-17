using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
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
            DxRectangle selectRange = new DxRectangle(points[0].X - 1, points[0].Y - 1, 10, 10);

            Stopwatch timer = Stopwatch.StartNew();

            KDTree<TestPoint> testTree = new KDTree<TestPoint>(testObject => testObject.Point, range, testObjects);
            TimeSpan treeBuildTime = timer.Elapsed;
            List<TestPoint> objectsInRange = testTree.InRange(selectRange);
            timer.Stop();

            Console.WriteLine(
                $"KDTree build time {treeBuildTime}, full query time {timer.Elapsed - treeBuildTime}, total {timer.Elapsed}");

            Assert.AreEqual(testObjects.Count, objectsInRange.Count);
        }
    }
}
