using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGameTest.Core.Utils.Distance
{

    public class TestPoint
    {
        public DxVector2 Point { get; }
        public UniqueId Id { get; }

        public TestPoint(DxVector2 point)
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
            var testObject = other as TestPoint;
            if (testObject != null)
            {
                return Id.Equals(testObject.Id);
            }
            return false;
        }
    }

    public static class DistanceHelper
    {
        public static List<DxRectangle> GenerateRectanglesInRange(DxRectangle range, int numPoints)
        {
            float xMin = range.Left;
            float yMin = range.Top;
            float xMax = range.Right;
            float yMax = range.Bottom;
            
            List<DxRectangle> generatedRectangles = new List<DxRectangle>(numPoints);
            for (int i = 0; i < numPoints; ++i)
            {
                float x = ThreadLocalRandom.Current.NextFloat(xMin, xMax);
                float y = ThreadLocalRandom.Current.NextFloat(yMin, yMax);
                float width = ThreadLocalRandom.Current.NextFloat(0, xMax - x);
                float height = ThreadLocalRandom.Current.NextFloat(0, yMax - y);
                DxRectangle rectangle = new DxRectangle(x, y, width, height);
                generatedRectangles.Add(rectangle);
            }
            return generatedRectangles;
        } 

        public static List<DxVector2> GeneratePointsInRange(DxRectangle range, int numPoints)
        {
            float xMin = range.Left;
            float yMin = range.Top;
            float xMax = range.Right;
            float yMax = range.Bottom;
            
            List<DxVector2> generatedPoints = new List<DxVector2>(numPoints);
            for (int i = 0; i < numPoints; ++i)
            {
                float x = ThreadLocalRandom.Current.NextFloat(xMin, xMax);
                float y = ThreadLocalRandom.Current.NextFloat(yMin, yMax);
                DxVector2 point = new DxVector2(x, y);
                generatedPoints.Add(point);
            }
            return generatedPoints;
        }
    }
}
