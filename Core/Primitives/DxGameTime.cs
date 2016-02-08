﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using ProtoBuf;

namespace DXGame.Core.Primitives
{
    /**
        Serializable wrapper for XNA GameTime 
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class DxGameTime : IEquatable<DxGameTime>, IComparable<DxGameTime>
    {
        [ProtoMember(1)]
        [DataMember]
        public TimeSpan TotalGameTime { get; }

        [ProtoMember(2)]
        [DataMember]
        public TimeSpan ElapsedGameTime { get; }

        [ProtoMember(3)]
        [DataMember]
        public bool IsRunningSlowly { get; }

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
            if(Check.IsNullOrDefault(other))
            {
                return 1;
            }
            return TotalGameTime.CompareTo(other.TotalGameTime);
        }
    }
}