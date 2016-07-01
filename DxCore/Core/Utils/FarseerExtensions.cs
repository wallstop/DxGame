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

        public static AABB Aabb(this DxRectangle space)
        {
            return new AABB(new Vector2(space.X, space.Y), new Vector2(space.X + space.Width, space.Y + space.Height));
        }

        // TODO
    }
}
