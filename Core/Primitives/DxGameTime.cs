using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Primitives
{
    /**
        Serializable wrapper for XNA GameTime 
    */

    [Serializable]
    [DataContract]
    public class DxGameTime : IEquatable<DxGameTime>
    {
        [DataMember]
        public TimeSpan TotalGameTime { get; private set; }

        [DataMember]
        public TimeSpan ElapsedGameTime { get; private set; }

        [DataMember]
        public bool IsRunningSlowly { get; private set; }

        public DxGameTime()
            : this(TimeSpan.Zero, TimeSpan.Zero)
        {
        }

        public DxGameTime(GameTime gameTime)
            : this(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly)
        {
        }

        public DxGameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
            : this(totalGameTime, elapsedGameTime, false)
        {
        }

        public DxGameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime, bool isRunningSlowly)
        {
            TotalGameTime = totalGameTime;
            ElapsedGameTime = elapsedGameTime;
            IsRunningSlowly = isRunningSlowly;
        }

        public bool Equals(DxGameTime other)
        {
            return TotalGameTime.Equals(other.TotalGameTime) && ElapsedGameTime.Equals(other.ElapsedGameTime) &&
                   IsRunningSlowly.Equals(other.IsRunningSlowly);
        }

        public GameTime ToGameTime()
        {
            return new GameTime(TotalGameTime, ElapsedGameTime, IsRunningSlowly);
        }

        public double DetermineScaleFactor(DxGame dxGame)
        {
            return (ElapsedGameTime.TotalMilliseconds * dxGame.TargetFps / DateTimeConstants.MILLISECONDS_PER_SECOND);
        }
    }
}