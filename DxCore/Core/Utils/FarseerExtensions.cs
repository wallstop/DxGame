using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Primitives;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Utils
{
    public static class FarseerExtensions
    {
        public static Vertices Vertices(this DxRectangle space)
        {
            return new Vertices(4)
            {
                new Vector2(space.X, space.Y),
                new Vector2(space.X + space.Width, space.Y),
                new Vector2(space.X + space.Width, space.Y + space.Height),
                new Vector2(space.X, space.Y + space.Height)
            };
        }

        public static List<DxVector2> DxVertices(this DxRectangle space)
        {
            return new List<DxVector2>(4)
            {
                new DxVector2(space.X, space.Y),
                new DxVector2(space.X + space.Width, space.Y),
                new DxVector2(space.X + space.Width, space.Y + space.Height),
                new DxVector2(space.X, space.Y + space.Height)
            };
        }

        public static Vertices ToVertices(this IEnumerable<DxVector2> source)
        {
            return ToVertices(source.Select(dxVector => dxVector.Vector2));
        }

        public static Vertices ToVertices(this IEnumerable<Vector2> source)
        {
            return new Vertices(source);
        }

        public static AABB ToAabb(this DxRectangle space)
        {
            return new AABB(new Vector2(space.X, space.Y), new Vector2(space.X + space.Width, space.Y + space.Height));
        }

        public static DxRectangle ToDxRectangle(this AABB aabb)
        {
            Vector2 lowerBounds = aabb.LowerBound;
            Vector2 upperBound = aabb.UpperBound;
            return new DxRectangle(lowerBounds, upperBound);
        }
    }
}
