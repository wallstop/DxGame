using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Wrappers
{
    [Serializable]
    [DataContract]
    public struct DxPoint : IEquatable<DxPoint>, IEquatable<Point>
    {

        public DxPoint(float x, float y)
        {
            
        }

        public bool Equals(DxPoint other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Point other)
        {
            throw new NotImplementedException();
        }
    }
}
