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
    internal class TestObject
    {
        public DxVector2 Point { get; }
        public UniqueId Id { get; }

        public TestObject(DxVector2 point)
        {
            Point = point;
            Id = new UniqueId();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var testObject = other as TestObject;
            if (testObject != null)
            {
                return Id.Equals(testObject.Id);
            }
            return false;
        }
    }

    public class QuadTreeTest
    {
        private static List<DxVector2> GeneratePointsInRange(DxRectangle range, int numPoints)
        {
            float xMin = range.Left;
            float yMin = range.Top;
            float xMax = range.Right;
            float yMax = range.Bottom;

            var rGen = new Random();
            List<DxVector2> generatedPoints = new List<DxVector2>(numPoints);
            for (int i = 0; i < numPoints; ++i)
            {
                float x = (float) rGen.NextDouble(xMin, xMax);
                float y = (float) rGen.NextDouble(yMin, yMax);
                DxVector2 point = new DxVector2(x, y);
                generatedPoints.Add(point);
            }
            return generatedPoints;
        }

        [Test]
        public void TestInRange()
        {
            var rGen = new Random();

            int numPoints = 10000;
            DxRectangle range = new DxRectangle(200, 250, 10000, 10000);
            List<DxVector2> points = GeneratePointsInRange(range, numPoints);

            List<TestObject> testObjects = points.Select(point => new TestObject(point)).ToList();


            Stopwatch timer = Stopwatch.StartNew();

            QuadTree<TestObject> testTree = new QuadTree<TestObject>(testObject => testObject.Point, range, testObjects);
            TimeSpan treeBuildTime = timer.Elapsed;
            List<TestObject> objectsInRange = testTree.InRange(range);
            timer.Stop();

            Console.WriteLine(
                $"Tree build time {treeBuildTime}, full query time {timer.Elapsed - treeBuildTime}, total {timer.Elapsed}");

            Assert.AreEqual(testObjects.Count, objectsInRange.Count);
        }
    }
}