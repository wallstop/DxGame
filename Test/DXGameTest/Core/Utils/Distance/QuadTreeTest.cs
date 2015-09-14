using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using NUnit.Framework;

namespace DXGameTest.Core.Utils.Distance
{
    public class QuadTreeTest
    {
        [Test]
        public void TestInRange()
        {
            int numPoints = 1000;
            DxRectangle range = new DxRectangle(200, 250, 10000, 10000);
            List<DxVector2> points = DistanceHelper.GeneratePointsInRange(range, numPoints);

            List<TestPoint> testObjects = points.Select(point => new TestPoint(point)).ToList();
            DxRectangle selectRange = range;

            Stopwatch timer = Stopwatch.StartNew();

            QuadTree<TestPoint> testTree = new QuadTree<TestPoint>(testObject => testObject.Point, range, testObjects);
            TimeSpan treeBuildTime = timer.Elapsed;
            List<TestPoint> objectsInRange = testTree.InRange(selectRange);
            timer.Stop();

            Console.WriteLine(
                $"QuadTree build time {treeBuildTime}, full query time {timer.Elapsed - treeBuildTime}, total {timer.Elapsed}");

            Assert.AreEqual(testObjects.Count, objectsInRange.Count);
        }

        // TODO: More distance tests
    }
}