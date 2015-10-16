using System;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Primitives
{
    public class DxCircleTest
    {
        private static readonly int MIN_CENTER_POINT = -1000000;
        private static readonly int MAX_CENTER_POINT = 1000000;
        private static readonly int MAX_RADIUS = 10000;

        [Test]
        public void TestCircleRectangleIntersectionCompleteIntersection()
        {
            TestUtils.RunMultipleTimes(() =>
            {
                var circle = GenerateCircle();
                var radius = circle.Radius;
                /* 
                    ...and gaurantee that if we place a Rectangle centered on the circle's center, 
                    that we will completely ellipse the circle (and thus intersect) 
                */
                var rectangleRadius = ThreadLocalRandom.Current.Next((int) Math.Ceiling(radius), (int) Math.Ceiling(radius + 1) * 2) + 1;

                var completelyCoveringRectangle = new DxRectangle(circle.Center.X - rectangleRadius,
                    circle.Center.Y - rectangleRadius, rectangleRadius * 2, rectangleRadius * 2);
                Assert.True(circle.Intersects(completelyCoveringRectangle), $"{circle}, {completelyCoveringRectangle}");
            });
        }

        [Test]
        public void TestCircleRectangleIntersectionPartialIntersection()
        {
            TestUtils.RunMultipleTimes(() =>
            {
                var circle = GenerateCircle();
                DxVector2 point;
                do
                {
                    point = GeneratePoint(circle.Center.X - circle.Radius, circle.Center.X + circle.Radius,
                        circle.Center.Y - circle.Radius, circle.Center.Y + circle.Radius);
                } while (!circle.Contains(point));
                var rectangle = GenerateRectangleFromCornerPoint(point);
                Assert.True(circle.Intersects(rectangle), $"{circle}, {rectangle}, {point}");
            });
        }

        [Test]
        public void TestCircleRectangleIntersectionNoIntersection()
        {
            TestUtils.RunMultipleTimes(() => { });
        }

        /* Generates an arbitrary Rectangle which has an arbitrary corner located at the provided point */

        private static DxRectangle GenerateRectangleFromCornerPoint(DxVector2 point)
        {
            var corner = ThreadLocalRandom.Current.Next(4);
            var width = ThreadLocalRandom.Current.Next(MAX_RADIUS) + 1;
            var height = ThreadLocalRandom.Current.Next(MAX_RADIUS) + 1;
            switch (corner)
            {
                /* Upper left corner */
                case 0:
                    return new DxRectangle(point.X, point.Y, width, height);
                /* Upper right corner */
                case 1:
                    return new DxRectangle(point.X - width, point.Y, width, height);
                /* Lower left corner */
                case 2:
                    return new DxRectangle(point.X, point.Y - height, width, height);
                /* Lower right corner */
                case 3:
                    return new DxRectangle(point.X - width, point.Y - height, width, height);
                default:
                    throw new InvalidOperationException(
                        $"Cannot determine a DxRectangle to make for corner number {corner}");
            }
        }

        private static DxVector2 GeneratePoint(float xMin, float xMax, float yMin, float yMax)
        {
            var x = ThreadLocalRandom.Current.NextFloat(xMin, xMax);
            var y = ThreadLocalRandom.Current.NextFloat(yMin, yMax);
            return new DxVector2(x, y);
        }

        /* Generates an arbitrary circle */

        private static DxCircle GenerateCircle()
        {
            var x = ThreadLocalRandom.Current.Next(MIN_CENTER_POINT, MAX_CENTER_POINT);
            var y = ThreadLocalRandom.Current.Next(MIN_CENTER_POINT, MAX_CENTER_POINT);
            var circleCenter = new DxVector2(x, y);
            var radius = ThreadLocalRandom.Current.Next(MAX_RADIUS) + 1;
            var circle = new DxCircle(circleCenter, radius);
            return circle;
        }
    }
}