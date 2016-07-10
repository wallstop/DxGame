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
        /**
            <summary>
                Given an enumeration of Rectangles, returns bundles of points representing simplified polygons
            </summary>
        */

        public static List<List<DxVector2>> Simplify(this IEnumerable<DxRectangle> bunchaRectangles)
        {
            /* 
                Algorithm is pretty simple: 
                    Remove all line segments that occur more than one time.
                    while(edges remaining)
                        Pick edge, remove
                        Walk shape, removing edges from the pool
                        When no more, finalize shape
                    Poop out some shapes

                TODO: Currently this converts rectangles with their corners touching
                (https://i.ytimg.com/vi/vfiSNCZE7Dg/hqdefault.jpg) into one "shape".
                We could do some smarter detection of polygons where we keep track of
                the outline that we've created so far and some smart puzzle-piece
                style creation with backtracking. But the time for that is not now.
            */
            List<DxLineSegment> shapeEdges =
                bunchaRectangles.SelectMany(rectangle => rectangle.Lines)
                    .GroupBy(lineSegment => lineSegment)
                    .Where(lineSegments => lineSegments.Count() == 1)
                    .SelectMany(lineSegment => lineSegment)
                    .ToList();

            List<List<DxVector2>> shapes = new List<List<DxVector2>>();
            List<DxVector2> currentShape = new List<DxVector2>();
            DxLineSegment? currentEdge = null;
            shapeWalkin: while(shapeEdges.Any() || currentShape.Any())
            {
                if(!currentEdge.HasValue)
                {
                    if(currentShape.Any())
                    {
                        shapes.Add(currentShape);
                        currentShape = new List<DxVector2>();
                        continue;
                    }
                    currentEdge = shapeEdges[shapeEdges.Count -1];
                    shapeEdges.RemoveAt(shapeEdges.Count -1);
                    continue;
                }

                currentShape.Add(currentEdge.Value.Start);

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
                        goto shapeWalkin;;
                    }
                }
                currentEdge = null;
            }

            return shapes;
        }
    }
}