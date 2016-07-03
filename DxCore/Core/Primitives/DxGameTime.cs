using System;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Primitives
{
    /**
        Serializable wrapper for XNA GameTime 
    */

    [Serializable]
    [DataContract]
    public class DxGameTime : IEquatable<DxGameTime>, IComparable<DxGameTime>
    {
        [DataMember]
        public TimeSpan TotalGameTime { get; private set;}
        
        [DataMember]
        public TimeSpan ElapsedGameTime { get; private set; }
        
        [DataMember]
        public bool IsRunningSlowly { get; private set; }

        [IgnoreDataMember]
        public double ScaleFactor
        {
            get
            {
                if(scale_ == 0)
                {
                    scale_ = Math.Min(1.0,
                        ElapsedGameTime.TotalMilliseconds * DxGame.Instance.TargetFps /
                        DateTimeConstants.MILLISECONDS_PER_SECOND);
                }
                return scale_;
            }
        }

        [NonSerialized] [IgnoreDataMember] private double scale_;

        public DxGameTime() : this(TimeSpan.Zero, TimeSpan.Zero) {}

        public DxGameTime(DxGameTime copy)
        {
            Validate.Hard.IsNotNullOrDefault(copy);
            TotalGameTime = copy.TotalGameTime;
            ElapsedGameTime = copy.ElapsedGameTime;
            IsRunningSlowly = copy.IsRunningSlowly;
        }

        public DxGameTime(GameTime gameTime)
            : this(gameTime.TotalGameTime, gameTime.ElapsedGameTime, gameTime.IsRunningSlowly) {}

        public DxGameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
            : this(totalGameTime, elapsedGameTime, false) {}

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

        /**

            <summary>
                Ordered naturally based on TotalGameTime
            </summary>
        */

        public int CompareTo(DxGameTime other)
        {
            if(Validate.Check.IsNullOrDefault(other))
            {
                return 1;
            }
            return TotalGameTime.CompareTo(other.TotalGameTime);
        }
    }
}