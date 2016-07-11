using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Primitives;

namespace DxCore.Core.Utils
{
    /**
        <summary>
            Provides some helpers for dealing with primitive data types in the Dx namespace
        </summary>
    */

    public static class PrimitiveExtensions
    {
        public static List<DxLineSegment> Edges(this IEnumerable<DxRectangle> bunchaRectangles)
        {
            /* 
                Algorithm is pretty simple: 
                    Remove all line segments that occur more than one time.
                    while(edges remaining)
                        Pick edge, remove
                        Walk shape, removing edges from the pool
                        When no more, finalize shape
                    Take finalized shape, turn into edge bundle
                    Poop that bad boy out
            */
            List<DxLineSegment> shapeEdges =
                bunchaRectangles.SelectMany(rectangle => rectangle.Lines)
                    .GroupBy(lineSegment => lineSegment)
                    .Where(lineSegments => lineSegments.Count() == 1)
                    .SelectMany(lineSegment => lineSegment)
                    .ToList();

            List<List<DxLineSegment>> shapes = new List<List<DxLineSegment>>();
            List<DxLineSegment> currentShape = new List<DxLineSegment>();
            DxLineSegment? currentEdge = null;
            shapeWalkin:
            while(shapeEdges.Any() || currentShape.Any())
            {
                if(!currentEdge.HasValue)
                {
                    if(currentShape.Any())
                    {
                        shapes.Add(currentShape);
                        currentShape = new List<DxLineSegment>();
                        continue;
                    }
                    currentEdge = shapeEdges[shapeEdges.Count - 1];
                    shapeEdges.RemoveAt(shapeEdges.Count - 1);
                    continue;
                }

                currentShape.Add(currentEdge.Value);

                for(int i = 0; i < shapeEdges.Count; ++i)
                {
                    DxLineSegment edgeToConsider = shapeEdges[i];
                    if(currentEdge.Value.End == edgeToConsider.Start)
                    {
                        currentEdge = edgeToConsider;
                        shapeEdges.RemoveAt(i);
                        goto shapeWalkin;
                    }
                    /* 
                        If we've met the end, my friend, we need to flip this poor line segment so we
                        maintain that we connect form start to end
                    */
                    if(currentEdge.Value.End == edgeToConsider.End)
                    {
                        currentEdge = edgeToConsider.Reverse;
                        shapeEdges.RemoveAt(i);
                        goto shapeWalkin;
                        ;
                    }
                }
                currentEdge = null;
            }

            /* 
                Shape bundles done! Now we turn them into vertical and horizontal edges.
                We can get away with this because we know they're rectangles instead of 
                arbitrary polygons.
            */
            return shapes.SelectMany(shape =>
            {
                List<DxLineSegment> horizontalEdges =
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    shape.Where(lineSegment => lineSegment.Start.Y == lineSegment.End.Y)
                        .GroupBy(lineSegment => lineSegment.Start.Y)
                        .Select(groupedLineSegment =>
                        {
                            float y = groupedLineSegment.Key;
                            SortedSet<float> xValues =
                                new SortedSet<float>(
                                    groupedLineSegment.SelectMany(segment => new[] {segment.Start.X, segment.End.X}));
                            return new DxLineSegment(new DxVector2(xValues.Min, y), new DxVector2(xValues.Max, y));
                        }).ToList();

                List<DxLineSegment> verticalEdges =
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    shape.Where(lineSegment => lineSegment.Start.X == lineSegment.End.X)
                        .GroupBy(lineSegment => lineSegment.Start.X)
                        .Select(groupedLineSegment =>
                        {
                            float x = groupedLineSegment.Key;
                            SortedSet<float> yValues =
                                new SortedSet<float>(
                                    groupedLineSegment.SelectMany(segment => new[] {segment.Start.Y, segment.End.Y}));
                            return new DxLineSegment(new DxVector2(x, yValues.Min), new DxVector2(x, yValues.Max));
                        }).ToList();
                return horizontalEdges.Concat(verticalEdges).ToList();
            }).ToList();
        }
    }
}